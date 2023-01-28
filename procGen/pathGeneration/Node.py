from dataclasses import dataclass


@dataclass
class Node:
    y: int
    x: int

    def __add__(self, other):
        return Node(self.y + other.y, self.x + other.x)

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def add(self, node):
        return Node(self.y + node.y, self.x + node.x)
