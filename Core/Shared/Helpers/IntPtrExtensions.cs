using System;
using System.Runtime.CompilerServices;

namespace ExileCore.Shared.Helpers
{
    /// <summary>
    /// Provides extension methods to validate and calculate with IntPtr.
    /// </summary>
    public static class IntPtrExtensions
    {
        /// <summary>
        /// Adds the value of the second pointer to the value of the first pointer.
        /// </summary>
        /// <param name="left">The pointer to add the second pointer to.</param>
        /// <param name="right">The pointer to add.</param>
        /// <returns>A new pointer that reflects the addition of two pointers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Add(this IntPtr left, IntPtr right)
        {
            return new IntPtr((long) left + (long) right);
        }

        /// <summary>
        /// Divides the value of the first pointer by the value of the second pointer.
        /// </summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor.</param>
        /// <returns>A new pointer that reflects the division of two pointers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Divide(this IntPtr left, IntPtr right)
        {
            return new IntPtr((long) left / (long) right);
        }

        /// <summary>
        /// Retrieves the value of a pointer.
        /// </summary>
        /// <param name="ptr">The pointer to retrieve its value from.</param>
        /// <returns>An unsigned long representing the value of the supplied pointer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetValue(this IntPtr ptr)
        {
            return (ulong) ptr;
        }

        /// <summary>
        /// Determines whether a pointer or offset is aligned.
        /// </summary>
        /// <param name="ptr">The pointer or offset to check.</param>
        /// <returns><see langword="true" /> if the supplied pointer or offset is aligned; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAligned(this IntPtr ptr)
        {
            var value = ptr.GetValue();

            return value == 1 || value % 2 == 0;
        }

        /// <summary>
        /// Determines whether the given pointer is not zero.
        /// </summary>
        /// <param name="ptr">The pointer or offset to check.</param>
        /// <returns><see langword="true" /> if the supplied pointer or offset is not zero; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotZero(this IntPtr ptr)
        {
            return ptr != IntPtr.Zero;
        }

        /// <summary>
        /// Determines whether the given pointer is a valid address on windows.
        /// </summary>
        /// <param name="ptr">The pointer to check.</param>
        /// <returns>
        /// <see langword="true" /> if the supplied pointer represents a valid address; otherwise,
        /// <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValid(this IntPtr ptr)
        {
            var value = (ulong) ptr;

            if (IntPtr.Size == 4)
                return value > 0x10000ul && value < 0xFFF00000ul;

            return value > 0x10000ul && value < 0x000F000000000000ul;
        }

        /// <summary>
        /// Determines whether a pointer or offset is zero.
        /// </summary>
        /// <param name="ptr">The pointer or offset to check.</param>
        /// <returns><see langword="true" /> if the supplied pointer is zero; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this IntPtr ptr)
        {
            return ptr == IntPtr.Zero;
        }

        /// <summary>
        /// Multiplies the value of the first pointer by the value of the second pointer.
        /// </summary>
        /// <param name="left">The first pointer.</param>
        /// <param name="right">The second pointer.</param>
        /// <returns>A new pointer that reflects the multiplication of two pointers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Multiply(this IntPtr left, IntPtr right)
        {
            return new IntPtr((long) left * (long) right);
        }

        /// <summary>
        /// Reduces the value of the first pointer by the value of the second pointer.
        /// </summary>
        /// <param name="left">The first pointer.</param>
        /// <param name="right">The second pointer.</param>
        /// <returns>A new pointer that reflects the subtraction of two pointers.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Subtract(this IntPtr left, IntPtr right)
        {
            return new IntPtr((long) left - (long) right);
        }
    }
}
