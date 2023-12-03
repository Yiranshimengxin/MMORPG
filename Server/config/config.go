package config

import (
	"fmt"
	"gopkg.in/yaml.v2"
	"os"
)

type Config struct {
	App *App   `yaml:"app"`
	Sql *MySql `yaml:"sql"`
}

type App struct {
	Host string `yaml:"host"`
}

type MySql struct {
	Host     string `yaml:"host"`
	Port     int    `yaml:"port"`
	Username string `yaml:"username"`
	Password string `yaml:"password"`
	Database string `yaml:"database"`
}

var ConfigData Config

func LoadConfig(file string) bool {
	data, err := os.ReadFile(file)
	if err != nil {
		panic(fmt.Sprintf("config path error: %v", err))
	}

	err = yaml.Unmarshal(data, &ConfigData)
	if err != nil {
		panic("unmarshal config.yml error")
	}

	return true
}
