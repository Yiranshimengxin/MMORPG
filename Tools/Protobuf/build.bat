
::protoc-24.1-win64\bin\protoc.exe -I=./ --csharp_out=. ./protocol.proto

protobuf-net\protogen.exe --csharp_out=. ./protocol.proto

protoc-24.1-win64\bin\protoc.exe -I=./ --go_out=. ./protocol.proto
protoc-24.1-win64\bin\protoc.exe -I=./ --go_out=. ./protocol_db.proto

protoc-24.1-win64\bin\protoc.exe -I=./ -o protocol.pb protocol.proto


copy .\Protocol.cs ..\..\client\Assets\Scripts\Net
copy .\protocol.pb.go ..\..\server\pb
copy .\protocol_db.pb.go ..\..\server\pb

pause