import argparse
import random
import struct
import array

parser = argparse.ArgumentParser(
    description="A tool to print matrix",
)

parser.add_argument(
    "file",
    help="A matrix file to print"
)

args = parser.parse_args()

with open(args.file, "rb") as file:
    ilen = struct.calcsize("i")
    dlen = struct.calcsize("d")
    
    rows = struct.unpack("i", file.read(ilen))[0]
    columns = struct.unpack("i", file.read(ilen))[0]
    
    for i in range(rows):
        for j in range(columns):
            value = struct.unpack("d", file.read(dlen))[0]
            print("{:05.2f}".format(value), end=" ")
        print()
        

