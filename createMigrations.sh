#!/bin/bash

if [[ -z $1 ]]; then echo "Please add a migration name"
else
  dotnet ef migrations add $1 --project AttackDefenseRunner
fi
