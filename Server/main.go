package main

import (
	"flag"
	"fmt"
	"game/config"
	"game/gate"
	"game/world"
	"os"
	"os/signal"
	"syscall"
	"time"
)

var (
	SigChan   = make(chan os.Signal, 1)
	closeChan chan struct{}
)

var (
	cfgPath = flag.String("config", "./config.yml", "config file path")
)

func main() {
	flag.Parse()
	config.LoadConfig(*cfgPath)

	//table.LoadAllTable()

	signal.Notify(SigChan, syscall.SIGHUP, syscall.SIGINT, syscall.SIGTERM, syscall.SIGQUIT)

	logicTicker := time.NewTicker(66 * time.Millisecond)
	closeChan = make(chan struct{})
	exitChan := make(chan struct{})
	go func() {
		defer func() {
			exitChan <- struct{}{}
		}()

		gs := gate.NewGate()
		go gs.Run()

		w := world.NewWorld()
		w.Init()

		for {
			select {

			case <-closeChan:
				goto QUIT
			case sig := <-SigChan:
				fmt.Printf("收到信号:%v\n", sig)
				close(closeChan)
			case <-logicTicker.C:
				w.Update()
			case client := <-gs.AcceptChan:
				w.OnAccept(client)
			case client := <-gs.CloseChan:
				w.OnClose(client)
			}
		}

	QUIT:
		gs.Stop()

		return
	}()
	<-exitChan
}
