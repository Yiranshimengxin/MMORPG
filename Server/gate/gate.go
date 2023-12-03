package gate

import (
	"fmt"
	"game/config"
	"game/network"
	"net"
	"sync"
	"sync/atomic"
)

func NewGate() *Gate {
	return &Gate{
		clientId:   0,
		AcceptChan: make(chan *Client, 100),
		CloseChan:  make(chan *Client, 100),
	}
}

// Gate 服务器
type Gate struct {
	listenr    net.Listener //监听器
	clientId   uint64       //客户端ID
	usersFD    sync.Map     //客户端连接
	AcceptChan chan *Client //接收客户端
	CloseChan  chan *Client //关闭客户端
}

// Run 运行服务器
func (g *Gate) Run() {
	l, e := net.Listen("tcp", config.ConfigData.App.Host)
	if e != nil {
		panic(e.Error())
	}
	g.listenr = l

	//启动网络服务器，接受客户端连接
	server := network.NewServer(g, &MsgProtocol{})
	go server.Start(l, func(conn net.Conn, i *network.Server) *network.Connection {
		return network.NewConnection(conn, server)
	})
}

// Stop 停止服务器
func (g *Gate) Stop() {
	err := g.listenr.Close()
	if err != nil {
		panic(err.Error())
		return
	}
}

// OnConnect 客户端连接
func (g *Gate) OnConnect(conn *network.Connection) bool {
	atomic.AddUint64(&g.clientId, 1)

	if conn.GetFD() == 0 {
		conn.SetFD(atomic.LoadUint64(&g.clientId))
	}

	client := NewClient(conn)
	g.AcceptChan <- client

	g.usersFD.Store(conn.GetFD(), client)

	return true
}

// OnClose 客户端关闭
func (g *Gate) OnClose(conn *network.Connection) {
	value, ok := g.usersFD.Load(conn.GetFD())
	if !ok {
		return
	}
	u := value.(*Client)
	u.onClose()
	g.usersFD.Delete(conn.SetFD)
}

// OnMessage 客户端消息
func (g *Gate) OnMessage(conn *network.Connection, pkg network.Packet) bool {
	fd := conn.GetFD()
	value, ok := g.usersFD.Load(fd)
	if ok {
		u := value.(*Client)
		u.onRequest(pkg)
	} else {
		fmt.Println("GateWork on message handshake error")
	}

	return true
}

// ReadMQ 获取客户端新建连接的管道
func (g *Gate) ReadMQ() <-chan *Client {
	return g.AcceptChan
}

// CloseMQ 获取客户端关闭连接的管道
func (g *Gate) CloseMQ() <-chan *Client {
	return g.CloseChan
}
