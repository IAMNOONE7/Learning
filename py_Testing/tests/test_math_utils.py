import math
import pytest

from src import math_utils


def test_add_basic():
    assert math_utils.add(2, 3) == 5
    assert math_utils.add(-1, 1) == 0
    assert math_utils.add(0, 0) == 0


def test_add_with_floats():
    result = math_utils.add(0.1, 0.2)
    # floats can be imprecise, so use isclose
    assert math.isclose(result, 0.3, rel_tol=1e-9)

@pytest.fixture
def math_context():
    # Could be a config object, database connection, etc.
    # For now it's just a dict to show the idea.
    return {"precision": 1e-9}

def test_add_uses_context(math_context):
    result = math_utils.add(0.1, 0.2)
    assert math.isclose(result, 0.3, rel_tol=math_context["precision"])


def test_divide_normal_case():
    assert math_utils.divide(10, 2) == 5
    assert math_utils.divide(-9, 3) == -3


def test_divide_by_zero_raises():
    # Example of testing exceptions
    with pytest.raises(ValueError) as exc_info:
        math_utils.divide(1, 0)

    assert "Division by zero" in str(exc_info.value)

@pytest.mark.parametrize(
    "n, expected",
    [
        (0, False),
        (1, False),
        (2, True),
        (3, True),
        (4, False),
        (5, True),
        (10, False),
        (11, True),
        (12, False),
        (13, True),
        (17, True),
    ],
)
def test_is_prime_parametrized(n, expected):
    assert math_utils.is_prime(n) == expected