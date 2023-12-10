package entity

import (
	"game/pb"
)

// 创建一个怪物
func NewMonster(id, hp, maxHp, level int32, x, y, z float32) *Monster {
	return &Monster{
		Attr: &Attr{
			ObjType:   ObjectEnemy,
			ID:        id,
			Hp:        hp,
			MaxHp:     maxHp,
			Level:     level,
			PositionX: x,
			PositionY: y,
			PositionZ: z,
			Damage:    10,
			Defence:   5,
		},
	}
}

// Monster 怪物
type Monster struct {
	*Attr
}

// GetID 获取ID
func (m *Monster) GetID() int32 { return m.ID }

// GetType 获取类型
func (m *Monster) GetType() int32 { return m.ObjType }

// Update 更新
func (m *Monster) Update(elapsedTime float32) {

}

func (m *Monster) ToPB() *pb.SceneObject {
	return &pb.SceneObject{
		Type:      m.ObjType,
		ObjId:     m.ID,
		Hp:        m.Hp,
		MaxHp:     m.MaxMp,
		PositionX: m.PositionX,
		PositionY: m.PositionY,
		PositionZ: m.PositionZ,
	}
}

// GetDamage 获取伤害
func (m *Monster) GetDamage() int32 { return m.Damage }

// IsDead 是否死亡
func (m *Monster) IsDead() bool { return m.Hp <= 0 }

// OnHit 被攻击
// 伤害计算公式：伤害=攻击力/（1+防御力/攻击力）
func (m *Monster) OnHit(attack Entity, cb AttackCallback) {
	damage := attack.GetDamage() / (1 + m.Defence/attack.GetDamage())
	m.Hp -= damage
	cb(m, damage)
	if m.Hp <= 0 {
		m.Dead(attack)
	}
}

// Dead 是否死亡
func (m *Monster) Dead(killer Entity) {

}
