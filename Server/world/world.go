package world

import (
	"database/sql"
	"errors"
	"fmt"
	"game/config"
	"game/db"
	"game/entity"
	"game/game"
	"game/gate"
	"game/pb"
	_ "github.com/go-sql-driver/mysql"
	"google.golang.org/protobuf/proto"
	"time"
)

// NewWorld 创建世界
func NewWorld() *World {
	return &World{
		clients:  make([]*gate.Client, 0),
		entities: make(map[int32]entity.Entity),
		players:  make(map[int64]entity.Entity),
		objectId: 0,
	}
}

// World 世界
type World struct {
	clients  []*gate.Client          //所有客户端
	entities map[int32]entity.Entity //所有实体
	players  map[int64]entity.Entity //所有玩家
	objectId int32                   //对象Id
	dbMySql  *sql.DB                 //mysql
}

// Init 初始化世界
func (w *World) Init() {
	//连接数据库
	dataSource := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?charset=utf8&parseTime=True",
		config.ConfigData.Sql.Username, config.ConfigData.Sql.Password, config.ConfigData.Sql.Host, config.ConfigData.Sql.Port, config.ConfigData.Sql.Database)
	myDB, err := sql.Open("mysql", dataSource)
	if err != nil {
		fmt.Println(err.Error())
		return
	}
	fmt.Println("connect mysql success")
	w.dbMySql = myDB

	//加载地图
	w.LoadMap()
}

// LoadMap 加载地图，这里是通过地图编辑工具导出地图信息，然后加载到世界中
func (w *World) LoadMap() {
	//create monster
	for i := 0; i < 1; i++ {
		//monster := entity.NewMonster(w.createObjectId(), 100, 100, 100, 0, 0, 0)
		//w.entities[monster.GetID()] = monster
	}
}

// Update 更新世界
func (w *World) Update() {
	for _, e := range w.entities {
		e.Update(0)
	}
}

// OnAccept 客户端连接
func (w *World) OnAccept(c *gate.Client) {
	c.RegisterMsgHandler(w)
	w.clients = append(w.clients, c)
}

// OnClose 客户端关闭
func (w *World) OnClose(c *gate.Client) {
	for _, e := range w.entities {
		if e.GetID() == c.GetPlayer().GetID() {
			delete(w.entities, e.GetID())
			delete(w.players, e.(*game.Player).GetUid())
		}
	}
}

// HandleMsg 处理消息
func (w *World) HandleMsg(c *gate.Client, session int32, name string, data []byte) {
	switch name {
	case "PBLoginReq":
		w.OnLogin(c, session, data)
	//case "PBEnterSceneReq":
	//	w.OnPlayerEnter(c, session, data)
	//case "PBAttackReq":
	//	w.OnAttack(c, session, data)
	//case "PBMoveReq":
	//	w.OnMove(c, session, data)
	//case "PBUseItemReq":
	//	w.OnUseItem(c, session, data)
	//case "PBSyncPositionReq":
	//	w.OnSyncPosition(c, session, data)
	default:
		fmt.Println("not find msg %s handler", name)
	}
}

// OnLogin 玩家登录
func (w *World) OnLogin(c *gate.Client, session int32, data []byte) {
	req := &pb.PBLoginReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	register := false
	dbUser := &db.User{}
	var userId int64
	// 判断有没有此账号，没有则新注册一个
	rows := w.dbMySql.QueryRow("select * from account where account = ?", req.GetAccount())
	var ac db.Account
	err = rows.Scan(&ac.Account, &ac.Uid)
	if errors.Is(err, sql.ErrNoRows) {
		var maxId int64
		w.dbMySql.QueryRow("select * from user id").Scan(&maxId)
		userId = maxId + 1
		w.dbMySql.Exec("update user id set max_id = ?", userId)

		// 插入account与id映射
		_, err := w.dbMySql.Exec("insert into account(account,uid) values(?,?)", req.GetAccount(), userId)
		if err != nil {
			fmt.Println(err.Error())
			return
		}

		// 插入user到db
		dbUser.Uid = userId
		dbUser.Name = req.GetAccount()
		dbUser.CreateTime = time.Now().Unix()
		dbUser.LastLoginTime = time.Now().Unix()
		_, err = w.dbMySql.Exec("insert into user(uid,name,createTime,lastLoginTime,lastLogoutTime,exp,level,positionX,positionY,positionZ,money,killNum,deadNum) values(?,?,?,?,?,?,?,?,?,?,?,?,?)",
			dbUser.Uid, dbUser.Name, dbUser.CreateTime, dbUser.LastLoginTime, dbUser.LastLogoutTime, 0, 1, 0, 0, 0, 0, 0, 0)
		if err != nil {
			fmt.Println(err.Error())
			return
		}

		//插入背包数据
		_, err = w.dbMySql.Exec("insert into bag(uid,data) values (?,?)", dbUser.Uid, []byte{})
		if err != nil {
			fmt.Println(err.Error())
			return
		}

		register = true
	} else {
		uRows := w.dbMySql.QueryRow("select * from user where uid = ?", ac.Uid)
		err = uRows.Scan(&dbUser.Uid, &dbUser.Name, &dbUser.CreateTime, &dbUser.LastLoginTime, &dbUser.LastLogoutTime, &dbUser.Exp, &dbUser.Level, &dbUser.PositionY, &dbUser.PositionY, &dbUser.PositionZ, &dbUser.Money, &dbUser.KillNum, &dbUser.DeadNum)
		if err != nil {
			fmt.Println(err.Error())
			return
		}
	}

	p := game.NewPlayer(w.CreateObjectId(), dbUser, w.dbMySql)
	w.entities[p.GetID()] = p
	w.players[p.GetUid()] = p
	c.AttachPlayer(p)
	p.OnLogin(register)

	rsp := &pb.PBLoginRsp{}
	rsp.Uid = dbUser.Uid
	rsp.Name = req.GetAccount()
	//for k,v:=range p.GetBag().GetItem(){
	//	rsp.Items=append(rsp.Items,&pb.PBItem{
	//		ItemId: k,
	//		Count: v,
	//	})
	//}

	c.Send("PBLoginRsp", session, rsp)
}

// CreateObjectId 生成场景里实例的唯一ID
func (w *World) CreateObjectId() int32 {
	w.objectId += 1
	return w.objectId
}
