PROJECT = BulletDevil

#can be linux-x64 OR win-x64
PLATFORM = linux-x64

BIN = bin/Debug/net10.0
PUB = bin/Release/net10.0/$(PLATFORM)/publish

VERSION = 0.0.1

SEPARATOR = --------------------------------

build:
	dotnet build
	@rm -rf ./$(BIN)/resources
	@cp -R ./resources ./$(BIN)
#	@cp config.json ./$(BIN)
	@echo $(SEPARATOR)

run:
	@dotnet ./$(BIN)/$(PROJECT).dll

test:
	dotnet build
	@rm -rf ./$(BIN)/resources
	@cp -R ./resources ./$(BIN)
#	@cp config.json ./$(BIN)
	@echo $(SEPARATOR)
	@dotnet ./$(BIN)/$(PROJECT).dll

publish:
	dotnet publish -c Release -r $(PLATFORM) -p:PublishSingleFile=true
	@rm -rf ./$(PUB)/resources
	@cp -R ./resources ./$(PUB)
#	@cp config.json ./$(PUB)
	@echo $(SEPARATOR)

clean:
	@rm -rf ./$(BIN)

purge:
	@rm -rf ./bin
