using Xunit;

public class IteratorTests : BaseTests
{
    [Fact]
    public void TestIteratorInterface ()
    {
        string example = @"
<?php
class MyIterator implements Iterator
{
    private $var = array();

    public function __construct($array)
    {
        if (is_array($array)) {
            $this->var = $array;
        }
    }

    public function rewind()
    {
        echo 'rewinding\n';
        reset ($this->var);
    }

    public function current ()
    {
        $var = current ($this->var);
        echo 'current: '.$var.""\n"";
        return $var;
    }

    public function key ()
    {
        $var = key ($this->var);
        echo 'key: '.$var.""\n"";
        return $var;
    }

    public function next ()
    {
        $var = next ($this->var);
        echo 'next: '.$var.""\n"";
        return $var;
    }

    public function valid ()
    {
        $key = key ($this->var);
        $var = ($key !== NULL && $key !== FALSE);
        echo 'valid: '.$var.""\n"";
        return $var;
    }

}

$values = array (1,2,3);
$it = new MyIterator ($values);

foreach ($it as $a => $b) {
    print $a.': '.$b.""\n"";
}
";

        Assert.Equal (_trimLines (@"
rewinding
valid: 1
current: 1
key: 0
0: 1
next: 2
valid: 1
current: 2
key: 1
1: 2
next: 3
valid: 1
current: 3
key: 2
2: 3
next:
valid:
"), _trimLines (_eval (example)));

    }

}
