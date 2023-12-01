package db

// Account 账号数据
type Account struct {
	Account string //账号
	Uid     int64
}

// User 用户数据
type User struct {
	Uid            int64   //ID
	Name           string  //名字
	CreateTime     int64   //创建时间
	LastLogoutTime int64   //最后下线时间
	LastLoginTime  int64   //登录时间
	Exp            int64   //经验
	Level          int32   //等级
	PositionX      float32 //位置
	PositionY      float32 //位置
	PositionZ      float32 //位置
	Money          int64   //金钱
	KillNum        int32   //杀敌数
	DeadNum        int32   //死亡数
}

// Item 物品
type Item struct {
	ItemId int32 //物品ID
	Num    int32 //数量
}

// ItemBag 物品背包
type ItemBag struct {
	Item []*Item
}
