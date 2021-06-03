Hacked together in 30 mins code :-)

Encrypt arg[0] using the provided keypair; outputs first the bytes, then newline x 2 and the b64 encoded version of the bytes to std:out

    sodiumencrypt "I LOVE BEES" "public.key" "private.key"

Decrypt arg[3] using the provided keypair then print the plaintext to std:out

    sodiumencrypt "I LOVE BEES" "public.key" "private.key" "ApfhgGRsdy0rx+lzV9P3rAqL3yRfqmmYcfDX6/mYKCR0vgj9j2OI/Pds4X4BjKBZaNMvvnUsCQ30B30="


