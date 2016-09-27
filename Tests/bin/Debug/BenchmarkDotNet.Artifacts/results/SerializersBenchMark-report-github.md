```ini

BenchmarkDotNet=v0.9.7.0
OS=Microsoft Windows NT 10.0.10586.0
Processor=Intel(R) Core(TM) i7-2600 CPU 3.40GHz, ProcessorCount=8
Frequency=3331525 ticks, Resolution=300.1628 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=64-bit RELEASE [RyuJIT]
JitModules=clrjit-v4.6.1080.0

Type=SerializersBenchMark  Mode=Throughput  

```
   Method |     Median |    StdDev |
--------- |----------- |---------- |
 ProtoBuf | 10.2220 us | 1.0464 us |
   Native | 28.8816 us | 0.5874 us |
