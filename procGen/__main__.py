import config
from config import *
from pathGeneration import generate_map_data
import argparse

argparser = argparse.ArgumentParser()
argparser.add_argument("-gw", "--gridWidth", default=20, type=int)
argparser.add_argument("-gh", "--gridHeight", default=20, type=int)
argparser.add_argument("-maxhi", "--maxHoleIndent", default=10, type=int)
argparser.add_argument("-minhi", "--minHoleIndent", default=5, type=int)
argparser.add_argument("-mpl", "--minPathLength", default=15, type=int)
argparser.add_argument("-minbp", "--minBoostPads", default=1, type=int)
argparser.add_argument("-maxbp", "--maxBoostPads", default=3, type=int)
argparser.add_argument("-minc", "--minComplicate", default=3, type=int)
argparser.add_argument("-maxc", "--maxComplicate", default=4, type=int)

args = argparser.parse_args()

Config.GRID_WIDTH = args.gridWidth
Config.GRID_HEIGHT = args.gridHeight
Config.MAX_HOLE_INDENT = args.maxHoleIndent
Config.MIN_HOLE_INDENT = args.minHoleIndent
Config.MIN_PATH_LENGTH = args.minPathLength
Config.MIN_BOOST_PAD_COUNT = args.minBoostPads
Config.MAX_BOOST_PAD_COUNT = args.maxBoostPads
Config.MIN_COMPLICATE = args.minComplicate
Config.MAX_COMPLICATE = args.maxComplicate

grid = [["." for i in range(Config.GRID_WIDTH)] for ii in range(Config.GRID_HEIGHT)]

map_data = generate_map_data(grid)

path = map_data[0]
boost_pads = map_data[1]
barriers = map_data[2]

write_text = "x,y\n"
for i in range(len(path)):
    write_text += f"{path[i].x},{path[i].y}" + ("\n" if i < len(path) - 1 else "")

with open("../path.csv", "w") as f:
    f.write(write_text)

write_text = "x,y\n"
for i in range(len(boost_pads)):
    write_text += f"{boost_pads[i].pos.x},{boost_pads[i].pos.y}" + ("\n" if i < len(boost_pads) - 1 else "")

with open("../boost.csv", "w") as f:
    f.write(write_text)