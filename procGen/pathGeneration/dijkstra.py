from config import *
from pathGeneration import Node


def get_neighbour_nodes(node, graph):
    neighbours = []
    #for i in [Node(0, -1), Node(0, 1), Node(-1, 0), Node(1, 0), Node(-1, -1), Node(1, 1), Node(1, -1), Node(-1, 1)]:
    for i in [Node(0, -1), Node(0, 1), Node(-1, 0), Node(1, 0)]:
        test_node = i + node
        test_node.parent = node
        if is_node_valid(test_node, graph):
            neighbours.append(test_node)
    return neighbours


def is_node_valid(node, graph):
    return GRID_WIDTH > node.x > -1 and GRID_HEIGHT > node.y > -1 and not graph[node.y][node.x]


def dijkstra(graph, start: Node, end: Node):
    visited = []
    node = start
    node.parent = None
    to_visit = [node]
    while True:
        neighbours = []

        for i in to_visit:
            for ii in get_neighbour_nodes(i, graph):
                if ii not in neighbours and ii not in visited:
                    if ii == end:
                        return get_path(ii)
                    neighbours.append(ii)

        if not neighbours:
            return []
        for i in to_visit:
            visited.append(i)
        to_visit = neighbours[::]


def get_path(node):
    path = []
    while node.parent:
        path.append(node)
        node = node.parent
    path.append(node)
    return path


def find_path(graph, start, end):
    def get_binary(var):
        return int(var == "#")

    maze = [list(map(get_binary, i)) for i in graph]

    path = dijkstra(maze, start, end)
    return path


