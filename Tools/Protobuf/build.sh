#!/bin/bash
set -x
set -e

DIR="$( cd "$( dirname "$0"  )" && pwd  )"
export GOPATH=$DIR
export PATH="$DIR:$PATH"


./protoc-24.1-osx-aarch_64/bin/protoc -I=$GOPATH --proto_path=$GOPATH/ --go_out=$GOPATH --go_opt=paths=source_relative $GOPATH/protocol.proto

./protoc-24.1-osx-aarch_64/bin/protoc --proto_path=. --csharp_out=. protocol.proto
