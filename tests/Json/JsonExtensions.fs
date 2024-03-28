module JsonExtensions 

let private f2 i = 
    if i < 10 then sprintf "0%i" i
    else sprintf "%i" i 

type System.DateTime with
    member this.ToJsonTimeString() = 
        $"{f2 this.Hour}:{f2 this.Minute}:{f2 this.Second}.{this.Millisecond}"

    member this.ToJsonDateString() = 
        $"{this.Year}-{f2 this.Month}-{f2 this.Day}"
        
    member this.ToJsonDateTimeString() = 
        $"{this.ToJsonDateString()}T{this.ToJsonTimeString()}Z"

module Time =
    
    let fromInts hour minute = 
        let d = System.DateTime(1,1,1,hour,minute,0)
        d.ToJsonTimeString()

module Date =
    
    let fromInts year month day = 
        let d = System.DateTime(year,month,day)
        d.ToJsonDateString()
      
module DateTime =
    
    let fromInts year month day hour minute = 
        let d = System.DateTime(year,month,day,hour,minute,0)
        d.ToJsonDateTimeString()