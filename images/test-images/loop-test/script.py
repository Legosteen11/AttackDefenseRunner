#!/usr/local/bin/python
import sys
from pwn import *
ip = sys.argv[1]

print("Executing with ip: {}".format(ip))
print("Can we also print another line???")