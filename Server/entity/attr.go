package entity

// 实体类型
const (
	ObjectNone   int32 = 0   //无效
	ObjectPlayer       = 101 //玩家
	ObjectNpc          = 102 //NPC
	ObjectEnemy        = 103 //敌人
)

// Attr 属性
type Attr struct {
	ObjType   int32   // 对象类型
	Name      string  // 名字
	ID        int32   // ID
	Hp        int32   // 血量
	MaxHp     int32   // 最大血量
	Mp        int32   // 蓝量
	MaxMp     int32   // 最大蓝量
	Level     int32   // 等级
	PositionX float32 // 位置
	PositionY float32 // 位置
	PositionZ float32 // 位置
	Damage    int32   // 伤害
	Defence   int32   // 防御
}
