#!/bin/bash
while :
do
  while read -r line
  do
    echo $(python script.py "$line")
  done < $1
done