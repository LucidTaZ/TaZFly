using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class HitpointsTest {

    [Test]
    public void RelativeHitpointsWorks()
    {
        //Arrange
		var hitpoints = new HitpointsController(10);
		Assert.True(hitpoints.IsAlive());

        //Act
		hitpoints.Decrease(5);

        //Assert
		Assert.Less(hitpoints.GetRelativeHitpoints() - 0.5f, Mathf.Epsilon);
    }
}
