package gate

import (
	"Server/network"
	"Server/pb"
	"fmt"
	"google.golang.org/protobuf/proto"
)

type MsgHandler interface {
	HandleMsg(c *Client, session int32, name string, data []byte)
}

func NewClient(conn *network.Connection) *Client {
	return &Client{
		conn: conn,
	}
}

// Client 客户端
type Client struct {
	conn *network.Connection
	//player *game.Player
	handler MsgHandler
}

//func (c* Client) AttachPlayer(p *game.Player){
//	c.player=p
//}

// GetFD 获取客户端连接的文件描述符
func (c *Client) GetFD() uint64 {
	return c.conn.GetFD()
}

// RegisterMsgHandler 注册消息处理器
func (c *Client) RegisterMsgHandler(handler MsgHandler) {
	c.handler = handler
}

func (c *Client) onClose() {

}

// Send 发送消息
func (c *Client) Send(name string, seesion int32, msg interface{}) {
	pbNetMsg := &pb.PBNetMsg{
		Proto:   name,
		Session: seesion,
	}

	switch v := msg.(type) {
	case proto.Message:
		if payload, err := proto.Marshal(v); err == nil {
			pbNetMsg.Payload = payload
		} else {
			return
		}
	case nil:
	default:
		return
	}

	//将消息发送给客户端
	retPack := NewPacket(pbNetMsg)
	err := c.conn.AsyncWritePacket(retPack, 0)
	if err != nil {
		c.conn.Close()
		fmt.Printf("gate user send msg:%v", err)
		return
	}
}

// onRequest 客户端请求
func (c *Client) onRequest(pkg network.Packet) bool {
	pakcet, _ := pkg.(*Packet)
	pbNetMsg := &pb.PBNetMsg{}
	err := pakcet.UnmarshalPB(pbNetMsg)
	if err != nil {
		return false
	}

	if c.handler != nil {
		c.handler.HandleMsg(c, pbNetMsg.GetSession(), pbNetMsg.GetProto(), pbNetMsg.GetPayload())
		return false
	} else {
		fmt.Println("msg handler is nil")
		return true
	}
}
