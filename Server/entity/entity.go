package entity

import "game/pb"

// 攻击回调
type AttackCallback func(e Entity, damage int32)

// Entity 实体接口
type Entity interface {
	GetID() int32
	GetType() int32
	GetDamage() int32
	IsDead() bool
	OnHit(attack Entity, cb AttackCallback)
	ToPB() *pb.SceneObject
	Update(elapsedTime float32)
}
