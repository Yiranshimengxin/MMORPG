package network

import (
	"errors"
	"net"
	"sync"
	"sync/atomic"
	"time"
)

// Error type
var (
	ErrConnClosing   = errors.New("use of closed network connection")
	ErrWriteBlocking = errors.New("write packet was blocking")
	ErrReadBlocking  = errors.New("read packet was blocking")
)

// Connection Conn exposes a set of callbacks for the various events that occur on a connection
type Connection struct {
	server            *Server
	conn              net.Conn      //the raw connection
	extraData         interface{}   //to save extra data
	closeOnce         sync.Once     //close to conn,once, per instance
	closeFlag         int32         //close flag
	closeChan         chan struct{} //close chanel
	packetSendChan    chan Packet   //packet send chanel
	packetReceiveChan chan Packet   //packet receive chanel
	callback          ConnCallback  //callback
	fd                uint64        //real socket fd
}

// ConnCallback is an interface of methods that are used as callbacks on a connection
type ConnCallback interface {
	//OnConnect is called when the connection was accepted,
	//If the return value of false is closed
	OnConnect(*Connection) bool

	//OnMessage is called when the connection receives a packet,
	//If the return value of false is closed
	OnMessage(*Connection, Packet) bool

	//OnClose is called when the connection closed
	OnClose(*Connection)
}

// NewConnection returns a wrapper of a raw conn
func NewConnection(conn net.Conn, server *Server) *Connection {
	c := &Connection{
		server:            server,
		callback:          server.callback,
		conn:              conn,
		closeChan:         make(chan struct{}),
		packetSendChan:    make(chan Packet, 1024),
		packetReceiveChan: make(chan Packet, 1024),
	}

	if s, ok := conn.(*net.TCPConn); !ok {
		panic("not tcp conn")
	} else {
		f, err := s.File()
		if err != nil {
			return c
		}
		c.fd = uint64(f.Fd())
	}
	return c
}

// GetExtraData gets the extra data from the Conn
func (c *Connection) GetExtraData() interface{} {
	return c.extraData
}

// PutExtraData puts the extra data with the Conn
func (c *Connection) PutExtraData(data interface{}) {
	c.extraData = data
}

// GetRawConn returns the raw net.TCPConn from the Conn
func (c *Connection) GetRawConn() net.Conn {
	return c.conn
}

func (c *Connection) GetFD() uint64 {
	return c.fd
}

func (c *Connection) SetFD(fd uint64) {
	c.fd = fd
}

// Close closes the connection
func (c *Connection) Close() {
	c.closeOnce.Do(func() {
		atomic.StoreInt32(&c.closeFlag, 1)
		close(c.closeChan)
		close(c.packetSendChan)
		close(c.packetReceiveChan)
		c.conn.Close()
		c.callback.OnClose(c)
	})
}

// IsClosed indicates whether the connection is closed
func (c *Connection) IsClosed() bool {
	return atomic.LoadInt32(&c.closeFlag) == 1
}

func (c *Connection) SetCallback(callback ConnCallback) {
	c.callback = callback
}

// AsyncWritePacket async write a packet, this method will never block
func (c *Connection) AsyncWritePacket(p Packet, timeout time.Duration) (err error) {
	if c.IsClosed() {
		return ErrConnClosing
	}

	defer func() {
		if e := recover(); e != nil {
			err = ErrConnClosing
		}
	}()

	if timeout == 0 {
		select {
		case c.packetSendChan <- p:
			return nil

		default:
			return ErrWriteBlocking
		}
	} else {
		select {
		case c.packetSendChan <- p:
			return nil

		case <-c.closeChan:
			return ErrConnClosing

		case <-time.After(timeout):
			return ErrWriteBlocking
		}
	}
}

// Do it
func (c *Connection) Do() {
	if !c.callback.OnConnect(c) {
		return
	}

	asyncDo(c.handleLoop, c.server.waitGroup)
	asyncDo(c.readLoop, c.server.waitGroup)
	asyncDo(c.writeLoop, c.server.waitGroup)

}

func (c *Connection) readLoop() {
	defer func() {
		recover()
		c.Close()
	}()

	for {
		select {
		case <-c.server.exitChan:
			return

		case <-c.closeChan:
			return

		default:
		}

		c.conn.SetReadDeadline(time.Now().Add(time.Second * 180))
		p, err := c.server.protocol.ReadPacket(c.conn)
		if err != nil {
			return
		}

		c.packetReceiveChan <- p
	}
}

func (c *Connection) writeLoop() {
	defer func() {
		recover()
		c.Close()
	}()

	for {
		select {
		case <-c.server.exitChan:
			return

		case <-c.closeChan:
			return

		case p := <-c.packetSendChan:
			if c.IsClosed() {
				return
			}
			c.conn.SetWriteDeadline(time.Now().Add(time.Second * 180))
			if _, err := c.conn.Write(p.Serialize()); err != nil {
				return
			}
		}
	}
}

func (c *Connection) handleLoop() {
	defer func() {
		recover()
		c.Close()
	}()

	for {
		select {
		case <-c.server.exitChan:
			return

		case <-c.closeChan:
			return

		case p := <-c.packetReceiveChan:
			if c.IsClosed() {
				return
			}
			if !c.callback.OnMessage(c, p) {
				return
			}
		}
	}
}

func asyncDo(fn func(), wg *sync.WaitGroup) {
	wg.Add(1)
	go func() {
		fn()
		wg.Done()
	}()
}
