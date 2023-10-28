.PHONY: all clean build yak spec update-manifest build-yak

YAK_PATH := /Applications/RhinoWIP.app/Contents/Resources/bin/yak
RELEASE_DIR := bin/release/net7
GITHUB_REPO_NAME := $(shell basename $(PWD))
PROJECT_FILE := RhAutoCADBridge.Mac.csproj

all: clean build yak

clean:
	dotnet clean $(PROJECT_FILE)

build:
	dotnet build -c release $(PROJECT_FILE)

yak: build
	@echo "Building with Yak..."
	@cd $(RELEASE_DIR) && \
		$(YAK_PATH) spec && \
		sed -i '' 's|<url>|https://github.com/$(GITHUB_REPO_NAME)|g' manifest.yml && \
		$(YAK_PATH) build --platform any
	@echo "Yak build completed."