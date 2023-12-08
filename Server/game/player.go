package game

import (
	"database/sql"
	"fmt"
	"game/db"
	"game/entity"
	"game/pb"
	"time"
)

// NewPLayer 创建玩家
func NewPlayer(id int32, db *db.User, mySql *sql.DB) *Player {
	p := &Player{
		Attr: &entity.Attr{
			ObjType: entity.ObjectPlayer,
			ID:      id,
		},
		db:      db,
		dbMySql: mySql,
		//bag:&PlayerBag{},
	}

	return p
}

// Player 玩家
type Player struct {
	*entity.Attr
	db      *db.User
	dirty   bool
	dbMySql *sql.DB
	bag     *Player
}

// OnLogin 登录游戏
func (p *Player) OnLogin(register bool) {
	p.db.LastLoginTime = time.Now().Unix()
	p.Level = p.db.Level
	p.PositionX = p.db.PositionX
	p.PositionY = p.db.PositionY
	p.PositionZ = p.db.PositionZ
	p.Hp = 100
	p.MaxHp = 100
	p.Mp = 100
	p.MaxMp = 100
	p.Damage = 10
	p.Defence = 5

	//p.bag.load(p,register)
}

// GetUid 获取玩家UID
func (p *Player) GetUid() int64 {
	return p.db.Uid
}

// GetID 获取玩家ID
func (p *Player) GetID() int32 {
	return p.ID
}

func (p *Player) GetDB() *db.User {
	return p.db
}

// GetType 获取类型
func (p *Player) GetType() int32 {
	return p.ObjType
}

// Update 更新
func (p *Player) Update(elapsedTime float32) {
	if p.dirty {
		p.Save()
	}
}

// GetPosition 获取位置
func (p *Player) GetPosition() (float32, float32, float32) {
	return p.PositionX, p.PositionY, p.PositionZ
}

func (p *Player) ToPB() *pb.SceneObject {
	return &pb.SceneObject{
		Type:      p.ObjType,
		ObjId:     p.ID,
		Hp:        p.Hp,
		MaxHp:     p.MaxHp,
		Mp:        p.Mp,
		MaxMp:     p.MaxMp,
		PositionX: p.PositionX,
		PositionY: p.PositionY,
		PositionZ: p.PositionZ,
		Level:     p.Level,
	}
}

// GetDamage 获取伤害
func (p *Player) GetDamage() int32 {
	return p.Damage
}

// IsDead 是否死亡
func (p *Player) IsDead() bool {
	return p.Hp <= 0
}

// OnHit 被攻击
// 伤害计算公式：伤害=攻击力/（1+防御力/攻击力）
func (p *Player) OnHit(attack entity.Entity, cb entity.AttackCallback) {
	damage := attack.GetDamage() / (1 + p.Defence/attack.GetDamage())
	p.Hp -= damage
	cb(p, damage)
	if p.Hp <= 0 {
		p.Dead(attack)
	}
}

// Dead 是否死亡
func (p *Player) Dead(killer entity.Entity) {
	p.Hp = 0
	p.db.DeadNum++
	p.SetDirty()
}

// AddExp 增加经验
func (p *Player) AddExp(exp int32) {
	p.db.Exp += int64(exp)

	p.Level += exp
	p.db.Level = p.Level
	p.SetDirty()
}

// AddMoney 增加金钱
func (p *Player) AddMoney(money int64) {
	p.db.Money += money
	p.SetDirty()
}

// SetPosition 设置位置
func (p *Player) SetPosition(x, y, z float32) {
	p.PositionX = x
	p.PositionY = y
	p.PositionZ = z
	p.SetDirty()
}

// Save 保存玩家数据
func (p *Player) Save() {
	p.db.PositionX = p.PositionX
	p.db.PositionY = p.PositionY
	p.db.PositionZ = p.PositionZ
	p.db.Level = p.Level

	_, err := p.dbMySql.Exec("update user set lastLogoutTime=?,lastLoginTime=?,exp=?,level=?,positionX=?,positionY=?,positionZ=?,money=?,killNum=?,deadNum=?"+"where uid=?",
		p.db.LastLogoutTime, p.db.LastLoginTime, p.db.Exp, p.db.Level, p.db.PositionX, p.db.PositionY, p.db.PositionZ, p.db.Money, p.db.KillNum, p.db.DeadNum, p.db.Uid)
	if err != nil {
		fmt.Println(err.Error())
		return
	}
	p.dirty = false

	p.bag.Save()
}

func (p *Player) SetDirty() {
	p.dirty = true
}
