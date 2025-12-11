from unittest.mock import MagicMock
from src.file_processor import read_file_lines, count_file_lines


def test_read_file_lines_magicmock():
    # Create a MagicMock representing a file-like object
    fake_file = MagicMock()

    # MagicMock already has __enter__ because it's a magic method.
    # We set __enter__ to return an object whose __iter__ returns an iterator.
    fake_file.__enter__.return_value.__iter__.return_value = iter([
        " first line\n",
        " second line\n",
        " third line\n",
    ])

    result = read_file_lines(fake_file)

    assert result == ["first line", "second line", "third line"]


def test_count_file_lines_magicmock():
    fake_file = MagicMock()

    fake_file.__enter__.return_value.__iter__.return_value = iter([
        " a\n",
        " b\n",
        " c\n",
        " d\n",
    ])

    count = count_file_lines(fake_file)

    assert count == 4


"""
    MagicMock is used here because the function under test uses:
    
    with file_obj as f:        -> requires file_obj.__enter__() and __exit__()
        for line in f:         -> requires file_obj.__iter__()
    
    __enter__ and __exit__ are special "magic methods" that make an object work
    inside a `with` statement (context manager). When Python sees:
    
          with something as x:
    
    it actually calls:
    
          something.__enter__()
          ...
          something.__exit__()
    
    __iter__ is another magic method that makes an object iterable, so that:
    
          for line in something:
    
    internally calls:
    
          iterator = something.__iter__()
   
    A normal Mock() does NOT have these magic methods, so mocking a file-like
    object would fail. MagicMock provides all magic methods by default, which is
    why it works perfectly for simulating files, context managers, and iteration.
"""

