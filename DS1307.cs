using Magellanic.I2C;
using System;

namespace Magellanic.Sensors.DS1307
{
    public class DS1307 : AbstractI2CDevice
    {
        private byte I2C_ADDRESS = 0x68;

        public DateTime GetCurrentTime()
        {
            byte[] readBuffer = new byte[7];

            this.Slave.WriteRead(new byte[] { 0x00 }, readBuffer);

            return ConvertByteBufferToDateTime(readBuffer);
        }

        public override byte[] GetDeviceId()
        {
            throw new NotImplementedException("This device does not have a unique device identifier.");
        }

        public override byte GetI2cAddress()
        {
            return I2C_ADDRESS;
        }

        public void SetDateTime(DateTime dateTime)
        {
            this.Slave.Write(ConvertTimeToByteArray(dateTime));
        }

        private int BinaryCodedDecimalToInteger(int value)
        {
            var lowerNibble = value & 0x0F;
            var upperNibble = value >> 4;

            var multipleOfOne = lowerNibble;
            var multipleOfTen = upperNibble * 10;

            return multipleOfOne + multipleOfTen;
        }

        private DateTime ConvertByteBufferToDateTime(byte[] dateTimeBuffer)
        {
            var second = BinaryCodedDecimalToInteger(dateTimeBuffer[0]);
            var minute = BinaryCodedDecimalToInteger(dateTimeBuffer[1]);
            var hour = BinaryCodedDecimalToInteger(dateTimeBuffer[2]);
            var dayofWeek = BinaryCodedDecimalToInteger(dateTimeBuffer[3]);
            var day = BinaryCodedDecimalToInteger(dateTimeBuffer[4]);
            var month = BinaryCodedDecimalToInteger(dateTimeBuffer[5]);
            var year = 2000 + BinaryCodedDecimalToInteger(dateTimeBuffer[6]);

            return new DateTime(year, month, day, hour, minute, second);
        }

        private byte[] ConvertTimeToByteArray(DateTime dateTime)
        {
            var dateTimeByteArray = new byte[8];

            dateTimeByteArray[0] = 0;
            dateTimeByteArray[1] = IntegerToBinaryCodedDecimal(dateTime.Second);
            dateTimeByteArray[2] = IntegerToBinaryCodedDecimal(dateTime.Minute);
            dateTimeByteArray[3] = IntegerToBinaryCodedDecimal(dateTime.Hour);
            dateTimeByteArray[4] = IntegerToBinaryCodedDecimal((byte)dateTime.DayOfWeek);
            dateTimeByteArray[5] = IntegerToBinaryCodedDecimal(dateTime.Day);
            dateTimeByteArray[6] = IntegerToBinaryCodedDecimal(dateTime.Month);
            dateTimeByteArray[7] = IntegerToBinaryCodedDecimal(dateTime.Year - 2000);

            return dateTimeByteArray;
        }

        private byte IntegerToBinaryCodedDecimal(int value)
        {
            var multipleOfOne = value % 10;
            var multipleOfTen = value / 10;

            // convert to nibbles
            var lowerNibble = multipleOfOne;
            var upperNibble = multipleOfTen << 4;

            return (byte)(lowerNibble + upperNibble);
        }
    }
}