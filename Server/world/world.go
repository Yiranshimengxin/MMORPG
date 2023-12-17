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
	for i := 0; i < 5; i++ {
		monster := entity.NewMonster(w.CreateObjectId(), 100, 100, 100, (float32)(i), 0, (float32)(i))
		w.entities[monster.GetID()] = monster
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
	case "PBEnterSceneReq":
		w.OnPlayerEnter(c, session, data)
	case "PBAttackReq":
		w.OnAttack(c, session, data)
	case "PBMoveReq":
		w.OnMove(c, session, data)
	case "PBUseItemReq":
		w.OnUseItem(c, session, data)
	case "PBSyncPositionReq":
		w.OnSyncPosition(c, session, data)
	default:
		fmt.Printf("not find msg %s handler\n", name)
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

	//判断账号是不是为空
	if req.GetAccount() == "" {
		fmt.Println("account is empty")
		return
	}

	register := false
	dbUser := &db.User{}
	var userId int64
	//判断有没有此账号，没有则新注册一个
	rows := w.dbMySql.QueryRow("select * from account where account =?", req.GetAccount())
	var ac db.Account
	err = rows.Scan(&ac.Account, &ac.Uid)
	if errors.Is(err, sql.ErrNoRows) {
		var maxId int64
		w.dbMySql.QueryRow("select * from user_id").Scan(&maxId)
		userId = maxId + 1
		w.dbMySql.Exec("update user_id set max_id= ?", userId)

		//插入account 与 id 映射
		_, err = w.dbMySql.Exec("insert into account(account,uid) values(?,?)", req.GetAccount(), userId)
		if err != nil {
			fmt.Println(err.Error())
			return
		}
	} else {
		uRows := w.dbMySql.QueryRow("select  * from user where uid = ?", ac.Uid)
		err = uRows.Scan(&dbUser.Uid, &dbUser.Name, &dbUser.CreateTime, &dbUser.LastLoginTime, &dbUser.LastLogoutTime, &dbUser.Exp, &dbUser.Level, &dbUser.PositionX,
			&dbUser.PositionY, &dbUser.PositionZ, &dbUser.Money, &dbUser.KillNum, &dbUser.DeathNum)

		userId = ac.Uid
		if errors.Is(err, sql.ErrNoRows) {
			//插入user到db
			dbUser.Uid = userId
			dbUser.Name = req.GetAccount()
			dbUser.CreateTime = time.Now().Unix()
			dbUser.LastLoginTime = time.Now().Unix()
			_, err = w.dbMySql.Exec("insert into user(uid,name,createTime,lastLoginTime,lastLogoutTime,exp,level,positionX,positionY,positionZ,money,killNum,deadNum)"+
				" values (?,?,?,?,?,?,?,?,?,?,?,?,?)",
				dbUser.Uid, dbUser.Name, dbUser.CreateTime, dbUser.LastLoginTime, dbUser.LastLogoutTime, 0, 1, 0, 0, 0, 0, 0, 0)
			if err != nil {
				fmt.Println(err.Error())
				return
			}
			// 插入背包数据
			_, err = w.dbMySql.Exec("insert into user_bag(uid,data) values (?,?)", dbUser.Uid, []byte{})
			if err != nil {
				fmt.Println(err.Error())
				return
			}
			register = true
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
	rsp.Money = p.GetDB().Money
	for k, v := range p.GetBag().GetItems() {
		rsp.Items = append(rsp.Items, &pb.PBItem{
			ItemId: k,
			Count:  v,
		})
	}

	c.Send("PBLoginRsp", session, rsp)
}

// CreateObjectId 生成场景里实例的唯一ID
func (w *World) CreateObjectId() int32 {
	w.objectId += 1
	return w.objectId
}

// OnPlayerEnter 玩家进入场景
func (w *World) OnPlayerEnter(c *gate.Client, session int32, data []byte) {
	req := &pb.PBEnterSceneReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	rsp := &pb.PBEnterSceneRsp{}
	for _, e := range w.entities {
		if e.GetID() == c.GetPlayer().GetID() {
			continue
		}
		rsp.SceneObjects = append(rsp.SceneObjects, e.ToPB())
	}

	self := c.GetPlayer()
	rsp.Self = self.ToPB()
	c.Send("PBEnterSceneRsp", session, rsp)

	notify := &pb.PBEnterSceneNotify{}
	notify.Object = self.ToPB()
	w.Broadcast("PBEnterSceneNotify", notify, self.GetID())
}

// Broadcast 广播消息
func (w *World) Broadcast(name string, msg interface{}, except int32) {
	for _, c := range w.clients {
		if c.GetPlayer().GetID() == except {
			continue
		}
		c.Send(name, 0, msg)
	}
}

// OnMove 玩家移动
func (w *World) OnMove(c *gate.Client, session int32, data []byte) {
	req := &pb.PBMoveReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	c.GetPlayer().SetPosition(req.GetPositionX(), req.GetPositionY(), req.GetPositionZ())

	notify := &pb.PBMoveNotify{}
	notify.ObjId = c.GetPlayer().GetID()
	notify.PositionX = req.GetPositionX()
	notify.PositionY = req.GetPositionY()
	notify.PositionZ = req.GetPositionZ()
	notify.Speed = req.GetSpeed()
	w.Broadcast("PBMoveNotify", notify, c.GetPlayer().GetID())
}

func (w *World) OnSyncPosition(c *gate.Client, session int32, data []byte) {
	req := &pb.PBSyncPositionReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	c.GetPlayer().SetPosition(req.GetPositionX(), req.GetPositionY(), req.GetPositionZ())
}

// OnAttack 玩家攻击
func (w *World) OnAttack(c *gate.Client, session int32, data []byte) {
	req := &pb.PBAttackReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	// 被攻击者
	targetId := req.GetObjId()
	target, ok := w.entities[targetId]
	if !ok {
		fmt.Printf("not find target %d", targetId)
		return
	}

	// 添加攻击距离检测

	target.OnHit(c.GetPlayer(), func(e entity.Entity, damage int32) {
		notify := &pb.PBAttackNotify{}
		notify.AttackObjId = c.GetPlayer().GetID()
		notify.AttackedObjId = targetId
		notify.Damage = damage
		w.Broadcast("PBAttackNotify", notify, 0)

		if e.IsDead() {
			delete(w.entities, e.GetID())
			if e.GetType() == entity.ObjectPlayer {
				delete(w.players, e.(*game.Player).GetUid())
			}

			c.GetPlayer().AddExp(10)
			c.GetPlayer().AddMoney(10)
			c.GetPlayer().AddItem(1001, 1)
			update := &pb.PBUpdateResourceNotify{
				Exp:   c.GetPlayer().GetDB().Exp,
				Money: c.GetPlayer().GetDB().Money,
				Level: c.GetPlayer().GetDB().Level,
			}
			update.Items = append(update.Items, &pb.PBItem{
				ItemId: 1001,
				Count:  c.GetPlayer().GetBag().GetItemNum(1001),
			})
			c.Send("PBUpdateResourceNotify", 0, update)

			notify := &pb.PBObjectDieNotify{}
			notify.ObjId = e.GetID()
			w.Broadcast("PBObjectDieNotify", notify, 0)
		}
	})
}

func (w *World) OnUseItem(c *gate.Client, session int32, data []byte) {
	req := &pb.PBUseItemReq{}
	err := proto.Unmarshal(data, req)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	c.GetPlayer().GetBag().OnItemUsed(req.GetItemId(), req.GetCount())

	rsp := &pb.PBUseItemRsp{}
	rsp.Items = append(rsp.Items, &pb.PBItem{
		ItemId: req.GetItemId(),
		Count:  c.GetPlayer().GetBag().GetItemNum(req.GetItemId()),
	})
	c.Send("PBUseItemRsp", session, rsp)
}
