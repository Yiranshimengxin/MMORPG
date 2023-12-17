package game

import (
	"fmt"
	"game/db"
	"game/pb"
	"game/table"
	"google.golang.org/protobuf/proto"
)

type PlayerBag struct {
	p      *Player
	dbData *db.ItemBag
	items  map[int32]int32
}

func (b *PlayerBag) Load(p *Player, register bool) {
	b.p = p
	b.dbData = &db.ItemBag{}
	b.items = make(map[int32]int32)
	if register {
		b.AddItem(1001, 10)
		b.AddItem(1002, 10)
		return
	}

	// 加载背包数据
	bagData := make([]byte, 1024*1024)
	uRows := b.p.dbMySql.QueryRow("select data from user_bag where uid = ?", b.p.GetUid())
	err := uRows.Scan(&bagData)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	// 反序列化背包数据
	pbBag := &pb.DBBag{}
	err = proto.Unmarshal(bagData, pbBag)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	// 转换成db.ItemBag
	b.dbData.Item = make([]*db.Item, 0)
	for _, item := range pbBag.Items {
		b.dbData.Item = append(b.dbData.Item, &db.Item{
			ItemId: item.Id,
			Num:    item.Num,
		})
		b.items[item.Id] = item.Num
	}
}

func (b *PlayerBag) Save() {
	pbBag := &pb.DBBag{}
	for id, num := range b.items {
		pbBag.Items = append(pbBag.Items, &pb.DBItem{
			Id:  id,
			Num: num,
		})
	}

	bagData, err := proto.Marshal(pbBag)
	if err != nil {
		fmt.Println(err.Error())
		return
	}

	_, err = b.p.dbMySql.Exec("update user_bag set data = ? where uid = ?", bagData, b.p.GetUid())
	if err != nil {
		fmt.Println(err.Error())
		return
	}
}

// GetItemNum 获取物品数量
func (b *PlayerBag) GetItemNum(itemId int32) int32 {
	return b.items[itemId]
}

// AddItem 添加物品
func (b *PlayerBag) AddItem(itemId, num int32) {
	b.items[itemId] += num
	b.Save()
}

// RemoveItem 移除物品
func (b *PlayerBag) RemoveItem(itemId, num int32) {
	b.items[itemId] -= num
	b.Save()
}

// OnItemUsed 物品可用
func (b *PlayerBag) OnItemUsed(itemId int32, num int32) {
	item := table.Item.GetItem(itemId)
	if item == nil {
		fmt.Println("item not exist")
		return
	}

	if b.items[itemId] < num {
		fmt.Println("item not enough")
		return
	}

	if item.CanBeUsed == 0 {
		fmt.Println("item can not be used")
		return
	}

	switch item.UseType {
	case 1:
		b.p.AddExp(item.UseValue * num)
	case 2:
		b.p.AddMoney(int64(item.UseValue * num))
	default:
		fmt.Println("item use type not exist")
	}

	b.RemoveItem(itemId, num)
}

func (b *PlayerBag) GetItems() map[int32]int32 {
	return b.items
}
