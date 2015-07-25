# Introduction #

[DotNetNuke](http://www.dotnetnuke.com) is an open source web application framework ideal for creating, deploying and managing interactive web, intranet and extranet sites. It is very well supported; just take a look at [SnowCovered](http://www.snowcovered.com). Unfortunately, VB and C# programmers don't cooperate as best they should, and take a mutually exclusive choice between the languages. This is unfortunate because [DotNetNuke](http://www.dotnetnuke.com)is a very well developed framework for ASP.NET that a lot of C# programmers do not want to look at.

## Purpose ##

In an attempt to open [DotNetNuke](http://www.dotnetnuke.com)to a wider audience, I have converted the original VB code into C#. Please support the authors at [DotNetNuke](http://www.dotnetnuke.com)for all their work, and report bugs in the C# code base to us, not them. To avoid forking the code, we will only update the code when a new release of the VB code is made available.

## Lessons Learned ##

Throughout the course of translating the code I learned many lessons. At first guess, you might think that translating the VB code would be easy, as did I. It wasn’t more than ten minutes into this project that I learned otherwise. First, I attempted to translate the code using VB Conversions and compiled the resultant source code with over 3,000 errors. This obviously wasn’t going to work out. Second, I tried to use Lutz Roeder’s Reflector to look at the code from the compiled binary. Although this resulted in slightly better results, it wasn’t a viable solution. If I was ever to complete this in time to be deemed useful I needed an alternative route. Finally I came up with a solution that was a combination of the first two methods, and some specialized tools I made specifically for this project to get it done.

## Results ##

As a result of converting the code, the new C# version runs incredibly much faster than its vb cousin. I was also able to increase its speed by compiling and generating a single named assembly for all pages + classes in the website. This wasn't that easy due to ASP.NET not allowing App\_GlobalResources in pre-compiled websites, but I was able to get around this bug by embedding the GlobalResources and SharedResources into the DotNetNuke.Library library, and using a ResourceLoader to extract the strings from the embedded resources in the assembly. The remaning config files were then placed into the config directory.


## Reasons ##

Productivity tools seem to be lacking for vb code which is not true for c# code. They help so much and I wanted to use them when working on the source code. I wanted to feel more in control of the codebase. I wanted to understand the framework better. And many, many more.

## Conclusion ##

I converted this project to make it more readable by myself and other C# programmers and open the DotNetNuke to a larger audience.

## History ##

Oct. 28, 2006 I released the first successful build in C#.
Jan. 1, 2006 I have fixed most of the bugs and have a workable copy of DNN in C#.