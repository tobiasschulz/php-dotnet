using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class ArrayTests : BaseTests
{
    private readonly string _defs;

    public ArrayTests ()
    {
        _defs = @"
            <?php
            $var1 = 'fck';
            $a1 = array( 'key1' => 'bla', 'key2' => 'blabla', 'key3' => $var1, );
            $a2 = array( 'key1' => $a1['key2'], 'key2' => $a1['key1'] );
        ";
    }

    [Fact]
    public void TestArrayAccess ()
    {
        _eval (_defs);
        Assert.Equal ("1", _eval (_defs + " $a1['key1'] == $a2['key2'] ? 1 : 0; "));
    }
}
