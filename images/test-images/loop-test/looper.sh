#!/bin/bash
echo "Starting exploit loop..."
while :
do
  while read -r line
  do
    echo $(python script.py "$line")
  done < $1
done