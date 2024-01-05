# Simple Command Line Multicast Testing Tool

**Download for Windows and Linux:** [https://github.com/enclave-networks/multicast-test/releases/tag/multicast-test-v1.0.1.0](https://github.com/enclave-networks/multicast-test/releases/tag/multicast-test-v1.0.1.0)

Testing multicast traffic can be challenging and tends to involve running an application on two systems, phsical or virtual connected to  the network.

Usually we might reach for iPerf and/or multicast video streaming in VLC. Both are useful but, iPerf3 has removed support for multicast traffic, and sometimes its not as obvious as it could be whether the tools are doing what we think they are. iPerf can be complicated, and VLC multicast streaming can be buggy.

![select an interface](https://github.com/enclave-networks/multicast-test/raw/master/select.png)

This is a simpler command-line tool that runs on both Linux and Windows which you can use to validate multicast connectivity. Run the tool on two or more different machines. Choose the relevant interface on both systems and then select action option 1 to transmit data, and action option 2 to recieve.

> Note. don't try using interface 0 (any) to send, it won't work. Pick the speicifc interface you want to test instead.

On the sending host you'll see output like this (option 1):

![sending data](https://github.com/enclave-networks/multicast-test/raw/master/sending.png)

![receiving data](https://github.com/enclave-networks/multicast-test/raw/master/receiving.png)

See also the [Singlewire Multicast Testing Tool](https://support.singlewire.com/s/software-downloads/a17C0000008Dg7AIAS/ictestermulticastzip) discussed [here](https://salmannaqvi.com/2016/11/14/simple-multicast-testing-tool-for-windows/) by Salman Naqvi – 2 x CCIE. The Singlewire tool is perfectly adequate if you have a single network interface, but if you're working on systems with multiple network interfaces, this version should be quite useful.
