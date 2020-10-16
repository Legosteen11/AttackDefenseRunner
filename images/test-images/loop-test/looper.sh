#!/bin/bash
while read -r line; do python script.py $line; done < $1