V2 is better!

Help:

	TODO

Generate keys:

    nacli.exe keygen -O "public.key" -o "private.key"

Example keys are used from now on so you should be able to run these commands as is.

Encrypt with output to the console/std:out encoded as a b64 string:

	nacli.exe encrypt -d "hello world" -K "ExampleKeys/public.key" -k "ExampleKeys/private.key" -f b64

Decrypt HEX-encoded data with output to the console/std:out as bytes:

    nacli.exe decrypt -e hex -d "096668C9211E00A855FAA74398CC49FC5D4E51EDE1FF38CF5FB9395B3CDA3E2B7102A5419542EDA01AFDBD91682F1698889C798D30FAF21FE9DA39" -K "ExampleKeys/public.key" -k "ExampleKeys/private.key"
	
Decrypt base64-encoded data with output to the console/std:out:

    nacli.exe decrypt -e b64 -d "c8L4ZL2Qkmf8wa9hJCYs9BOFqGjjqHE+E5IQBg5By34Vm8zvzsjWwHLG5S/GLnluGSTOwtHHtbaMfI4=" -K "ExampleKeys/public.key" -k "ExampleKeys/private.key"
