#!/bin/bash

docker run \
	-d \
	--name postgres_local \
	--net=host \
	--rm \
	-e POSTGRES_PASSWORD=adr \
	-e POSTGRES_USER=adr \
	postgres:12
	