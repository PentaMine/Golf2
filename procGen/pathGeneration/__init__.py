import config
from config import *
from random import randint

from pathGeneration.Node import Node
from pathGeneration.BoostPad import BoostPad
from pathGeneration.dijkstra import findPath
import numpy as np


class PathType:
    VERTICAL = 0
    HORIZONTAL = 1
    LEFT_TO_RIGHT = 2
    RIGHT_TO_LEFT = 3


def generate_map_data(graph: list[list[str]]):
    start, hole = None, None
    path = []
    while len(path) < MIN_PATH_LENGTH:
        nodes = generate_start_and_hole_pos()
        start = nodes[0]
        hole = nodes[1]
        path = findPath(graph, start, hole)

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"

    draw_path(graph, path)

    complicate_path(path, graph)
    erase_path(graph)

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"

    path = findPath(graph, start, hole)

    draw_path(graph, path)

    complicate_path(path, graph)
    erase_path(graph)

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"

    primary_path = findPath(graph, start, hole)

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"
    draw_path(graph, primary_path)

    secondary_path = make_forks(graph, path, hole, start)

    for i in secondary_path:
        primary_path.append(i)

    draw_path(graph, primary_path)

    boost_pads = get_boost_pad_positions(secondary_path, graph)
    barriers = get_barrier_positions(graph)

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"

    for i in graph:
        print(*i)

    return remove_duplicate_nodes(primary_path), boost_pads, barriers


def remove_duplicate_nodes(l):
    out = []
    for i in l:
        if i not in out:
            out.append(i)
    return out


def get_barrier_positions(graph):
    barriers = []
    for i in range(len(graph)):
        for ii in range(len(graph[i])):
            if graph[i][ii] == "#":
                barriers.append(Node(i, ii))
    return barriers


def erase_path(graph):
    for i in range(len(graph)):
        for ii in range(len(graph[i])):
            if graph[i][ii] == "+":
                graph[i][ii] = "."


def erase_barriers(graph):
    for i in range(len(graph)):
        for ii in range(len(graph[i])):
            if graph[i][ii] == "#":
                graph[i][ii] = "."


def draw_path(graph, path):
    for i in path[1:-1]:
        graph[i.y][i.x] = "+"


def convert_tuples_to_nodes(tuples):
    return [Node(i[0], i[1]) for i in tuples]


def generate_start_and_hole_pos():
    model = randint(0, 15)
    max_x = GRID_WIDTH - 1
    max_y = GRID_HEIGHT - 1

    model_binary = list(map(int, '{0:0b}'.format(model)))
    while len(model_binary) < 4:
        model_binary.insert(0, 0)

    start = Node(randint(MIN_HOLE_INDENT, MAX_HOLE_INDENT), randint(MIN_HOLE_INDENT, MAX_HOLE_INDENT))
    hole = Node(randint(MIN_HOLE_INDENT, MAX_HOLE_INDENT), randint(MIN_HOLE_INDENT, MAX_HOLE_INDENT))

    start.x = max_x - start.x if model_binary[0] else start.x
    hole.x = max_x - hole.x if model_binary[1] else hole.x

    start.y = max_y - start.y if model_binary[2] else start.y
    hole.y = max_y - hole.y if model_binary[3] else hole.y

    return start, hole


def complicate_path(path, graph):
    points = randint(2, 4)
    split_points = []
    for i in np.array_split(np.array(path), points)[:-1]:
        split_points.append(i[-1])
        graph[i[-1].y][i[-1].x] = "O"  # TODO: remove in prod
    split_path(split_points, graph)


def split_path(points, graph):
    for i in points:
        insert_barrier(i, get_path_type_at_point(i, graph), graph)
    pass


def get_path_type_at_point(point: Node, graph, is_random=True):
    positions = [point.add(i) for i in
                 [Node(0, -1), Node(0, 1), Node(-1, 0), Node(1, 0)]]
    horizontal = [PathType.HORIZONTAL, PathType.RIGHT_TO_LEFT, PathType.LEFT_TO_RIGHT]
    vertical = [PathType.VERTICAL, PathType.RIGHT_TO_LEFT, PathType.LEFT_TO_RIGHT]
    for i in positions[0:2]:
        if is_index_in_range_x(i.x) and is_index_in_range_y(i.y) and graph[i.y][i.x] == "+":
            return horizontal[randint(0, 2)] if is_random else horizontal[0]
    for i in positions[2:4]:
        if is_index_in_range_x(i.x) and is_index_in_range_y(i.y) and graph[i.y][i.x] == "+":
            return vertical[randint(0, 2)] if is_random else vertical[0]
    for i in positions[4:6]:
        if is_index_in_range_x(i.x) and is_index_in_range_y(i.y) and graph[i.y][i.x] == "+":
            return PathType.RIGHT_TO_LEFT
    for i in positions[6:8]:
        if is_index_in_range_x(i.x) and is_index_in_range_y(i.y) and graph[i.y][i.x] == "+":
            return PathType.LEFT_TO_RIGHT


def is_index_in_range_x(index):
    return 0 <= index < config.GRID_WIDTH


def is_index_in_range_y(index):
    return 0 <= index < config.GRID_HEIGHT


def insert_barrier(pos: Node, typ, graph):
    diffs = []
    match typ:
        case PathType.HORIZONTAL:
            diffs.append(Node(2, 0))
            diffs.append(Node(1, 0))
            diffs.append(Node(0, 0))
            diffs.append(Node(-1, 0))
            diffs.append(Node(-2, 0))
        case PathType.VERTICAL:
            diffs.append(Node(0, 2))
            diffs.append(Node(0, 1))
            diffs.append(Node(0, 0))
            diffs.append(Node(0, -1))
            diffs.append(Node(0, -2))

        case PathType.LEFT_TO_RIGHT:
            diffs.append(Node(2, 2))
            diffs.append(Node(1, 1))
            diffs.append(Node(0, 0))
            diffs.append(Node(-1, -1))
            diffs.append(Node(-2, -2))

            diffs.append(Node(3, 2))
            diffs.append(Node(2, 1))
            diffs.append(Node(1, 0))
            diffs.append(Node(0, -1))
            diffs.append(Node(-1, -2))

        case PathType.RIGHT_TO_LEFT:
            diffs.append(Node(-2, 2))
            diffs.append(Node(-1, 1))
            diffs.append(Node(0, 0))
            diffs.append(Node(1, -1))
            diffs.append(Node(2, -2))

            diffs.append(Node(-1, 2))
            diffs.append(Node(0, 1))
            diffs.append(Node(1, 0))
            diffs.append(Node(2, -1))
            diffs.append(Node(3, -2))

    for i in diffs:
        try:
            position = pos.add(i)
            graph[position.y][position.x] = "#"
        except IndexError:
            pass


def make_forks(graph, path, hole, start):
    distances = []
    for i in path:
        distances.append((abs(i.y - hole.y) + abs(i.x - hole.x) + 1) * (abs(i.y - start.y) + abs(i.x - start.x) + 1))
    point = path[distances.index(max(distances))]
    type = get_path_type_at_point(point, graph)

    match type:
        case PathType.HORIZONTAL:
            c = 0
            while True:
                try:
                    if graph[point.y + c][point.x] == "#":
                        break
                    graph[point.y + c][point.x] = "#"
                    c += 1

                except IndexError:
                    break
            c = 0
            while True:
                try:
                    if graph[point.y + c][point.x] == "#":
                        break
                    graph[point.y + c][point.x] = "#"
                    c -= 1
                except IndexError:
                    break
        case PathType.VERTICAL:
            c = 0
            while True:
                try:
                    if graph[point.y][point.x + c] == "#":
                        break
                    graph[point.y][point.x + c] = "#"
                    c += 1
                except IndexError:
                    break
            c = 0
            while True:
                try:
                    if graph[point.y][point.x + c] == "#":
                        break
                    graph[point.y][point.x + c] = "#"
                    c -= 1
                except IndexError:
                    break
        case PathType.LEFT_TO_RIGHT:
            c = 0
            while True:
                try:

                    if graph[point.y + c][point.x + c] == "#" or graph[point.y + c + 1][point.x + c] == "#":
                        break

                    graph[point.y + c][point.x + c] = "#"
                    graph[point.y + c + 1][point.x + c] = "#"
                    c += 1
                except IndexError:
                    break
            c = 0
            while True:
                try:
                    if graph[point.y + c][point.x + c] == "#" or graph[point.y + c - 1][point.x + c] == "#":
                        break
                    graph[point.y + c][point.x + c] = "#"
                    graph[point.y + c - 1][point.x + c] = "#"
                    c -= 1
                except IndexError:
                    break
        case PathType.RIGHT_TO_LEFT:
            c = 0
            while True:
                try:
                    if graph[point.y + c][point.x - c] == "#" or graph[point.y + c + 1][point.x - c] == "#":
                        break
                    graph[point.y + c][point.x - c] = "#"
                    graph[point.y + c + 1][point.x - c] = "#"
                    c += 1
                except IndexError:
                    break
            c = 0
            while True:
                try:
                    if graph[point.y - c][point.x + c] == "#" or graph[point.y - c - 1][point.x + c] == "#":
                        break

                    graph[point.y - c][point.x + c] = "#"
                    graph[point.y - c - 1][point.x + c] = "#"
                    c -= 1
                except IndexError:
                    break

    graph[start.y][start.x] = "@"
    graph[hole.y][hole.x] = "X"
    found_path = findPath(graph, start, hole)

    for i in path:
        found_path.append(i)

    return found_path


def get_boost_pad_positions(path, graph):
    profile_a = [Node(0, 1), Node(0, -1)]
    profile_b = [Node(1, 0), Node(-1, 0)]
    profile_c = [Node(1, 1), Node(-1, -1)]
    profile_d = [Node(-1, 1), Node(1, -1)]
    profiles = [profile_a, profile_b, profile_c, profile_d]
    point_profiles = []
    for i in path:
        for ii in range(len(profiles)):
            if i + profiles[ii][0] in path and i + profiles[ii][0] in path:
                point_profiles.append(ii)
                break

    num = randint(MIN_BOOST_PAD_COUNT, MAX_BOOST_PAD_COUNT)
    prev = None
    streak = 0
    streaks = []
    indexes = []
    inds = []
    for i in range(len(point_profiles)):
        if point_profiles[i] == prev:
            streak += 1
            inds.append(i)
        else:
            streaks.append(streak)
            indexes.append(inds)
            streak = 0
            inds = []
        prev = point_profiles[i]

    indexes = sorted(indexes, reverse=True, key=lambda x: len(x))[:num]
    indexes = [BoostPad(path[i[len(i) // 2]]) for i in indexes]
    return indexes
