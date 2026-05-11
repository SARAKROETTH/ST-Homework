using SmartPark.Core.Models;
using SmartPark.Core.Services;
using FsCheck;
using FsCheck.Xunit;

namespace SmartPark.Tests;

public class ParkingFeeCalculatorTests
{
    private readonly ParkingFeeCalculator _calculator = new();

    // ────────────────────────────────────────────────────────────
    //  EXAMPLE TEST — shows the naming convention and AAA pattern.
    //  Delete or keep this; it does not count toward your grade.
    // ────────────────────────────────────────────────────────────

    [Fact]
    public void CalculateFee_ZeroDuration_ReturnsFree()
    {
        // Arrange
        var checkIn = new DateTime(2026, 3, 16, 10, 0, 0);  // Monday
        var checkOut = checkIn; // same time = 0 duration

        // Act
        var result = _calculator.CalculateFee(VehicleType.Car, MembershipTier.Guest, checkIn, checkOut);

        // Assert
        Assert.Equal(0m, result.TotalFee);
    }

    #region Verify CheckIn before CheckOut
    [Fact]
    public void Verify_CheckOut_Before_checkIn()
    {
        // Arragne
        var checkIn = new DateTime(2025,12,12,1,0,0);
        var checkOut = checkIn.AddHours(1);
        // act
        var result = Assert.Throws<ArgumentException>(() =>
        {
            _calculator.CalculateFee(
            VehicleType.Car,
            MembershipTier.Platinum,
            checkIn,
            checkOut
        );
        });
    
        // Assert
        Assert.Equal("Checkin before checkOut",result.Message);
    }

    #endregion

    #region Basic Fee Calculation
    // Test basic hourly rates for each vehicle type
    // Consider using [Theory] with [InlineData] for multiple scenarios
    [Theory]
    [InlineData(VehicleType.Car,1,1000)]
    [InlineData(VehicleType.Motorcycle,1,500)]
    [InlineData(VehicleType.SUV,1,1500)]
    public void Calculate_Basic_Fee(
        VehicleType vehicleType,
        int hour,
        decimal expectValue
    )
    {
        // Arrange
        var checkIn = new DateTime(2026,12,10,1,0,0 );
        var checkOut = checkIn.AddHours(hour);

        // Act
        var result  = _calculator.CalculateFee(vehicleType,MembershipTier.Guest,checkIn,checkOut);
        // Assert 
        Assert.Equal(expectValue,result.BaseFee);
        
    }    
    #endregion


    #region Duration Rounding
    // Test how partial hours are rounded for billing
    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public void Calculate_Duration_BillableHours(
        int hour
    )
    {
        // Arrange 
        var checkIn = new DateTime(2025,12,12,1,0,0);

        var checkOut = checkIn.AddHours(hour);
        // Act
        var result  =  _calculator.CalculateFee(VehicleType.Car,MembershipTier.Guest,checkIn,checkOut);

        // Assert 
        Assert.Equal(hour,result.TotalFee);
    }

    #endregion

    #region Grace Period
    // Test the free parking window and its boundaries
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(30)]
    public void Calculate_Grace_Period_ReturnFree(
        int minute
    )
    {
        //Arrange 
        var checkIn = new DateTime(2026,5,11,3,0,0,0);
        var checkOut = checkIn.AddMinutes(minute);

        // Act 
        var result = _calculator.CalculateFee(VehicleType.Motorcycle,MembershipTier.Guest,checkIn,checkOut);

        // Assert 
        Assert.Equal("Free",result.Breakdown);
        
    }
    #endregion

    #region Daily Cap
    // Test that fees respect maximum daily limits per vehicle typep>te


    #endregion

    #region Overnight Fee
    // Test the flat fee applied for sessions that extend into late hours
    #endregion

    #region Weekend Surcharge
    // Test the percentage-based surcharge on specific days
    #endregion

    #region Holiday Surcharge
    // Test holiday pricing and its interaction with weekend pricing
    #endregion

    #region Membership Discounts
    // Test discount tiers and what amounts they apply to
    #endregion

    #region Lost Ticket
    // Test the penalty and how it interacts with other fee modifiers
    [Theory]
    [InlineData(VehicleType.Car, default(MembershipTier))]
    public void Calculate_Lost_Ticket(
        VehicleType vehicleType,
        MembershipTier membershipTier
    ){

        // Arrange
        var checkIn = new DateTime(2026,5,11,1,0,0);
        var checkOut = checkIn.AddHours(1);

        // Act
        var result = _calculator.CalculateFee(vehicleType,membershipTier,checkIn,checkOut,true);

        // Assert 
        Assert.Equal(20000m,result.TotalFee);

    }
    
    #endregion

    #region Edge Cases
    // Test invalid inputs and boundary conditions


    #endregion

    #region Property-Based Tests
    // Write at least 5 FsCheck properties that must hold for ALL valid inputs

    // You may need custom Arbitrary<T> for generating valid DateTime pairs
    #endregion
}
