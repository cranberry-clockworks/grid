import argparse
import random
import struct
import array

parser = argparse.ArgumentParser(
    description="A tool to generate matrix",
)

parser.add_argument(
    "output",
    help="the path to file containing the generated matrix"
)
parser.add_argument(
    "--rows",
    type=int,
    required=True,
    help="number of rows of the generated matrix"
)
parser.add_argument(
    "--columns",
    type=int,
    required=True,
    help="number of columns of the generated matrix"
)
parser.add_argument(
    "-z",
    "--zero",
    required=False,
    action="store_true",
    help="generate matrix filled with nils"
)

args = parser.parse_args()

value = 1.0

with open(args.output, "wb") as file:
    file.write(struct.pack("i", args.rows))
    file.write(struct.pack("i", args.columns))
    if args.zero:
        for row in range(args.rows):
            file.write(array.array("d", [0] * args.columns))
    else:
        for row in range(args.rows):
            for cell in range(args.columns):
                file.write(struct.pack("d", value))
                value += 1
