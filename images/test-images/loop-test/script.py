#!/usr/local/bin/python
import sys
from pwn import *
ip = sys.argv[1]

print("Executing with ip: {}".format(ip))