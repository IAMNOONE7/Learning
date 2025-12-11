def read_file_lines(file_obj):
    """
    Reads all lines from a file-like object inside a 'with' block.
    Returns a list of stripped lines.
    """
    lines = []

    with file_obj as f:
        for line in f:
            lines.append(line.strip())

    return lines


def count_file_lines(file_obj):
    """
    Uses read_file_lines() and returns the number of lines in the file.
    """
    return len(read_file_lines(file_obj))
