from config import *
from pathGeneration import generate_map_data

grid = [["." for i in range(GRID_WIDTH)] for ii in range(GRID_HEIGHT)]

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
"""
write_text = "x,y\n"
for i in range(len(barriers)):
    write_text += f"{barriers[i].x},{barriers[i].y}" + ("\n" if i < len(barriers) - 1 else "")
with open("../barrier.csv", "w") as f:
    f.write(write_text)
"""