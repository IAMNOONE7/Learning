import math
import pytest

from src import param_examples as pe

"""
Pytest Parametrization & Fixtures — Explanation

1. @pytest.mark.parametrize
   --------------------------------------------
   Parametrization allows you to run the same test function multiple times
   with different inputs. Instead of writing many almost-identical tests,
   you define a list of parameters and pytest will generate a test case for
   each one.

   Example:
       @pytest.mark.parametrize("x", [1, 2, 3])
       def test_positive(x):
           assert x > 0

   This will run 3 tests:
       test_positive[1]
       test_positive[2]
       test_positive[3]

2. @pytest.mark
   --------------------------------------------
   Pytest "marks" are labels you attach to tests. `parametrize` is one kind of mark,
   but marks are also used for:
     - skipping tests
     - marking tests as slow
     - categorizing tests (e.g., "integration", "api")
     - applying behavior to a whole test class

   Useful when running selective sets of tests:
       pytest -m "not slow"
       pytest -m "api"


3. @pytest.fixture
   --------------------------------------------
   Fixtures provide reusable, injectable test data or setup logic.
   A fixture returns a value, and any test that includes the fixture name
   as a function argument automatically receives that value.

   Example:
       @pytest.fixture
       def db_connection():
           return create_test_db()

       def test_query(db_connection):
           assert db_connection.is_connected 

   Parametrized fixture example:
       @pytest.fixture(params=[1, 2, 3])
       def number(request):
           return request.param

       def test_numbers(number):
           assert number in [1, 2, 3]

In short:
- @pytest.mark.parametrize -> generate multiple test cases
- @pytest.mark -> label or categorize tests
- @pytest.fixture -> reusable test dependencies and setup logic

These three features are the foundation of powerful, flexible,
and maintainable test suites in pytest.
"""


# ---------------------------------------------------------------------------
# 1) Basic parametrization: one parameter
# ---------------------------------------------------------------------------

@pytest.mark.parametrize("value", [1, 2, 100, 999])
def test_is_positive_basic(value):
    """Single-parameter parametrization."""
    assert pe.is_positive(value)


# ---------------------------------------------------------------------------
# 2) Multiple parameters (most common pattern)
# ---------------------------------------------------------------------------

@pytest.mark.parametrize(
    "a, b, expected",
    [
        (2, 3, 5),
        (-1, 1, 0),
        (10, -5, 5),
    ],
)
def test_add_multiple_params(a, b, expected):
    """Parametrize multiple arguments and expected result."""
    assert pe.add(a, b) == expected


# ---------------------------------------------------------------------------
# 3) Parametrized IDs for nicer test names
# ---------------------------------------------------------------------------

@pytest.mark.parametrize(
    "text, expected_count",
    [
        pytest.param("hello world", 2, id="two-words"),
        pytest.param("one", 1, id="single-word"),
        pytest.param("", 0, id="empty-string"),
        pytest.param("one  two   three", 3, id="irregular-spaces"),
    ],
)
def test_word_count_with_ids(text, expected_count):
    """
    Parametrization with explicit IDs to make test output more readable.
    """
    assert pe.word_count(text) == expected_count


# ---------------------------------------------------------------------------
# 4) Parametrizing exception vs normal result
# ---------------------------------------------------------------------------

@pytest.mark.parametrize(
    "a, b, expected",
    [
        (10, 2, 5.0),
        (9, 3, 3.0),
        pytest.param(5, 0, ValueError, id="division-by-zero"),
    ],
)
def test_safe_divide_parametrized(a, b, expected):
    """
    Mix cases: sometimes we expect a value, sometimes an exception.
    """
    if expected is ValueError:
        with pytest.raises(ValueError):
            pe.safe_divide(a, b)
    else:
        result = pe.safe_divide(a, b)
        assert math.isclose(result, expected)


# ---------------------------------------------------------------------------
# 5) Parametrized fixture: fixture generates values for tests
# ---------------------------------------------------------------------------

@pytest.fixture(params=[1, 2, 3, 10])
def positive_number(request):
    """
    Fixture that will be run for each parameter in 'params'.
    Tests using this fixture will run once per value.
    """
    return request.param


def test_positive_number_fixture(positive_number):
    """
    This test runs 4 times: for 1, 2, 3, 10.
    """
    assert pe.is_positive(positive_number)


# ---------------------------------------------------------------------------
# 6) Parametrizing a whole test class
# ---------------------------------------------------------------------------

@pytest.mark.parametrize("value", [-1, 0, 1, 2])
class TestIsPositiveClass:
    """
    Every test method in this class will run once per 'value'.
    So both tests below will run 4 times (total 8 test cases).
    """

    def test_is_positive_flag(self, value):
        # expected True only if value > 0
        assert pe.is_positive(value) == (value > 0)

    def test_is_positive_not_for_negative(self, value):
        # negative numbers should be False
        if value < 0:
            assert pe.is_positive(value) is False


# ---------------------------------------------------------------------------
# 7) Indirect parametrization: parameters go into fixture
# ---------------------------------------------------------------------------

@pytest.fixture
def user(request):
    """
    'user' fixture that builds a user dict based on request.param.
    The parameter passed from @pytest.mark.parametrize is not the
    final user object, but configuration for this fixture.
    """
    name, active = request.param
    return pe.make_user(name=name, active=active)


@pytest.mark.parametrize(
    "user",
    [
        ("Alice", True),
        ("Bob", False),
        ("Charlie", True),
    ],
    indirect=True,  # <--- tell pytest to send these to the 'user' fixture
)
def test_indirect_user_fixture(user):
    """
    The 'user' parameter here is the result of the 'user' fixture,
    not the raw tuple. 'indirect=True' passes the tuples to the
    fixture via request.param, and the fixture builds the dict.
    """
    assert "name" in user
    assert "active" in user
    assert isinstance(user["name"], str)
    assert isinstance(user["active"], bool)


# ---------------------------------------------------------------------------
# 8) Combining multiple @parametrize decorators (Cartesian product)
# ---------------------------------------------------------------------------

@pytest.mark.parametrize("a", [1, 2])
@pytest.mark.parametrize("b", [10, 20])
def test_multiply_cartesian(a, b):
    """
    This test runs 4 times with combinations:
        (b=10, a=1)
        (b=20, a=1)
        (b=10, a=2)
        (b=20, a=2)

    Note: pytest applies the decorators from bottom to top.
    """
    result = pe.multiply(a, b)
    assert result == a * b


# Another Cartesian example with strings
@pytest.mark.parametrize("prefix", ["dev", "prod"])
@pytest.mark.parametrize("suffix", ["api", "web"])
def test_combine_strings(prefix, suffix):
    """
    Cartesian product again:
        prefix in [dev, prod]
        suffix in [api, web]
    """
    combined = pe.combine_strings(prefix, suffix)
    assert combined == f"{prefix}-{suffix}"
