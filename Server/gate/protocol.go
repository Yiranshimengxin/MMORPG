package gate

import (
	"Server/network"
	"encoding/binary"
	"errors"
	"google.golang.org/protobuf/proto"
	"io"
)

const (
	DataLen      = 4                  //对应协议头字节长度
	MinPacketLen = DataLen            //最小包长度
	MaxPacketLen = (2 << 8) * DataLen //最大包长度
)

// Packet 包
type Packet struct {
	len  uint32
	data []byte
}

// GetData 获取数据
func (p *Packet) GetData() []byte {
	return p.data
}

// Serialize 序列化
func (p *Packet) Serialize() []byte {
	dataLen := uint32(len(p.data))

	//申请一个最小包长度的字节切片
	buff := make([]byte, MinPacketLen+dataLen)

	//将数据长度写入到buff中，以大端字节序写入
	binary.BigEndian.PutUint32(buff, dataLen)

	//将数据写入到buff中
	if dataLen > 0 {
		copy(buff[MinPacketLen:], p.data)
	}
	return buff
}

// UnmarshalPB 反序列化
func (p *Packet) UnmarshalPB(msg proto.Message) error {
	return proto.Unmarshal(p.data, msg)
}

// NewPacket 创建一个包
func NewPacket(msg interface{}) *Packet {
	p := &Packet{}

	switch v := msg.(type) {
	case []byte:
		p.data = v
		p.len = uint32(len(p.data))
	case proto.Message:
		if mdata, err := proto.Marshal(v); err == nil {
			p.data = mdata
			p.len = uint32(len(p.data))
		} else {
			return nil
		}
	case nil:
	default:
		return nil
	}

	return p
}

type MsgProtocol struct {
}

// ReadPacket 读取包
func (p *MsgProtocol) ReadPacket(r io.Reader) (network.Packet, error) {
	buff := make([]byte, MinPacketLen)

	//data length
	if _, err := io.ReadFull(r, buff); err != nil {
		return nil, err
	}
	dataLen := binary.BigEndian.Uint32(buff)

	if dataLen > MaxPacketLen {
		return nil, errors.New("data max")
	}

	//id
	msg := &Packet{
		len: dataLen,
	}

	//data
	if msg.len > 0 {
		msg.data = make([]byte, msg.len)
		if _, err := io.ReadFull(r, msg.data); err != nil {
			return nil, err
		}
	}

	return msg, nil
}
