# Small N Stats Demand Calculator
Small N Stats Demand Calculator is a WPF application that uses R interop libraries (R.Net Community) to easily facilitate complex calculations required of behavior economic (BE) scientists.  

Features include:
  - Non-linear model fittings (John Nash's modified LM optimizer, specificially suited for BE models)
  - Multiple Models available (Hursh & Silberberg's Exponential Model, Koffarnus et. al.'s Exponentiated model)
  - R-based graphical output in appropriate log space(s) (ggplot2)
  - Easily imports common file formats into the application's spreadsheet (.xlsx, .csv)
  - Full range of BE outcome metrics (empirical and derived), saveable in common spreadsheet file formats

### Version
1.0.0.32

### Referenced Works (F/OSS software)
The Small N Stats Demand Calculator uses a number of open source projects to work properly:
* R Statistical Package - GPL v2 Licensed. Copyright (C) 2000-16. The R Core Team
* RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved [Github](https://github.com/jmp75/rdotnet)
* nlmrt R Package - GPLv2 Licensed. Copyright (C) 2016. John C. Nash
* ExponentialDemandFunction R script - GPLv2 Licensed. Copyright (c) 2016. Shawn Gilroy [Github](https://github.com/miyamot0/ExponentialDemandFitting)
* ExponentiatedDemandFunction R script - GPLv2 Licensed. Copyright (c) 2016. Shawn Gilroy [Github](https://github.com/miyamot0/ExponentiatedDemandFitting)
* nlstools R Package - GPLv2 Licensed. Copyright(C) 2015 Florent Baty and Marie-Laure Delignette - Muller, with contributions from Sandrine Charles, Jean - Pierre Flandrois, and Christian Ritz
* ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham
* reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham
* beezdemand Package R package - GPLv2 Licensed. Copyright (c) 2016, Brent Kaplan [Github](https://github.com/bkaplan4/beezdemand)

### Referenced Works (academic works)
The Small N Stats Demand Calculator is based on the following academic works:
* Hursh, S. R. and Silberberg, A. (2008). Economic demand and essential value. Psychological Review, 115, 186–198.
* Koffarnus, M. N., Franck, C. T., Stein, J. and Bickel, W. K. (2015). A modified exponential behavioral economic demand model to better describe consumption data. Experimental Clinical Psychopharmacology, 23, 504-512.

### Acknowledgements and Credits
* Brent Kaplan, Applied Behavioral Economics Laboratory, University of Kansas (www.behavioraleconlab.com) [Github](https://github.com/bkaplan4)
* Derek D. Reed, Applied Behavioral Economics Laboratory, University of Kansas (www.behavioraleconlab.com) [Github](https://github.com/derekdreed)
* Chris Franck, Laboratory for Interdisciplinary Statistical Analysis - Virginia Tech

### Installation
You will need the R open-source statistical package for model fitting/charting to be performed.
Once DemandCalculator is installed, it will perform a one-time install the necessary R packages (internet required).
DemandCalculator is a ClickOnce application, the program will automatically update as the program is refined.

### Development
Want to contribute? Great! Emails or PM's are welcome.

### Todos
* Beta testing

### License
----
GPL-Version 2
