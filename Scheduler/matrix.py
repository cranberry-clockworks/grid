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
    "--width",
    type=int,
    required=True,
    help="width of the generated matrix"
)
parser.add_argument(
    "--height",
    type=int,
    required=True,
    help="height of the generated matrix"
)
parser.add_argument(
    "-z",
    "--zero",
    required=False,
    action="store_true",
    help="generate matrix filled with nils"
)

args = parser.parse_args()

with open(args.output, "wb") as file:
    file.write(struct.pack("i", args.width))
    file.write(struct.pack("i", args.height))
    if args.zero:
        for row in range(args.height):
            file.write(array.array("d", [0] * args.width))
    else:
        for row in range(args.height):
            for cell in range(args.width):
                file.write(struct.pack("d", random.uniform(-1e6, 1e6)))
