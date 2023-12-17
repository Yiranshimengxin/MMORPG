package table

type RowItem struct {
	Id        int32 //物品ID
	Type      int32 //类型 1：消耗品 2：装备
	CanBeUsed int32 //是否可以使用 0：不可使用 1：可使用
	UseType   int32 //使用类型 0：无 1：使用后增加经验 2：使用后增加金钱
	UseValue  int32 //使用值
}

type ItemDataTable struct {
	items map[int32]*RowItem
}

func (m *ItemDataTable) Init() {
	m.items = make(map[int32]*RowItem)

	m.items[1001] = &RowItem{
		Id:        1001,
		Type:      1,
		CanBeUsed: 1,
		UseType:   1,
		UseValue:  10,
	}

	m.items[1002] = &RowItem{
		Id:        1002,
		Type:      1,
		CanBeUsed: 1,
		UseType:   2,
		UseValue:  10,
	}
}

func (m *ItemDataTable) GetItem(id int32) *RowItem {
	return m.items[id]
}
