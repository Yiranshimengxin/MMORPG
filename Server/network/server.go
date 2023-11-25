package network

import (
	"net"
	"sync"
)

type Server struct {
	callback  ConnCallback    //message callbacks in connection
	protocol  Protocol        //customize packet protocol
	exitChan  chan struct{}   //notify all goroutines to shut down
	waitGroup *sync.WaitGroup //wait for all goroutines
	closeOnce sync.Once
	listener  net.Listener
}

// NewServer create a server
func NewServer(callback ConnCallback, protocol Protocol) *Server {
	return &Server{
		callback:  callback,
		protocol:  protocol,
		exitChan:  make(chan struct{}),
		waitGroup: &sync.WaitGroup{},
	}
}

type ConnectionCreator func(net.Conn, *Server) *Connection

// Start starts service
func (s *Server) Start(listener net.Listener, create ConnectionCreator) {
	s.listener = listener
	s.waitGroup.Add(1)
	defer func() {
		s.waitGroup.Done()
	}()

	for {
		select {
		case <-s.exitChan:
			return

		default:
		}

		//accept new connection from client
		conn, err := listener.Accept()
		if err != nil {
			continue
		}

		s.waitGroup.Add(1)
		go func() {
			create(conn, s).Do()
			s.waitGroup.Done()
		}()
	}
}

// Stop stops service
func (s *Server) Stop(wait bool) {
	s.closeOnce.Do(func() {
		close(s.exitChan)
		s.listener.Close()
	})
	if wait {
		s.waitGroup.Wait()
	}
}
