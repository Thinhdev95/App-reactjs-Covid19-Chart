import React, {useState, useEffect } from 'react'
import { Pie, defaults, Doughnut } from 'react-chartjs-2'
import {TextField, FormControl, InputLabel, Select, NativeSelect, FormHelperText, Button} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';

const BarChart = () => {
  const UrlApi = 'https://disease.sh/v3/covid-19/countries/vietnam';
  const UrlApi_covid_all = 'https://disease.sh/v3/covid-19/all';
  const [chartData, setChartData] = useState({});
  const [chartDataAll, setchartDataAll] = useState({});
  const [todayCases_All, settodayCases_All] = useState('');
  const [todayCases, settodayCases] = useState('');

  const chart = () => {
        let Title_Push = ["Ca nhiễm", "hồi phục", "tử vong"];
        let Data_Push = [];
        fetch(UrlApi)
           .then(res => res.json())
            .then(res => {
                Data_Push.push(parseInt(res.cases))
                Data_Push.push(parseInt(res.recovered))
                Data_Push.push(parseInt(res.deaths))
                settodayCases(res.todayCases)
            console.log(res);
            console.log(Title_Push);
            console.log(Data_Push);
            setChartData({
              labels: Title_Push,
              datasets: [
                {
                  label: "level of thiccness",
                  data: Data_Push,
                  backgroundColor: [ "rgba(75, 22, 192, 0.6)","rgba(75, 192, 43, 0.6)","red",
                ],
                  borderWidth: 1
                }
              ]
            });
          })
          .catch(err => {
            console.log(1123);
            console.log(err);
          });
  };
  const chart_covid_all = () => {
    let Title_Push = ["Ca nhiễm", "hồi phục", "tử vong"];
    let Data_Push = [];
    fetch(UrlApi_covid_all)
       .then(res => res.json())
        .then(res => {
            Data_Push.push(parseInt(res.cases))
            Data_Push.push(parseInt(res.recovered))
            Data_Push.push(parseInt(res.deaths))
            settodayCases_All(res.todayCases)
        console.log(res);
        console.log(Title_Push);
        console.log(Data_Push);
        setchartDataAll({
          labels: Title_Push,
          datasets: [
            {
              label: "level of thiccness",
              data: Data_Push,
              backgroundColor: [ "rgba(75, 22, 192, 0.6)","rgba(75, 192, 43, 0.6)","red",
            ],
              borderWidth: 1
            }
          ]
        });
      })
      .catch(err => {
        console.log(1123);
        console.log(err);
      });
};
  //set 
  useEffect(() => {
    chart(); chart_covid_all();
  },[]);
  return (
   <div style={{textAlign:"center", display:"flex"}}>
        <div style={{width: "350px",height:"300px",margin:"0 auto"}}>
        <h2>Việt Nam </h2>
      <Doughnut
        data={chartData}
        options={{
          legend: { display: true, position: "right" },

          datalabels: {
            display: true,
            color: "white",
          },
          tooltips: {
            backgroundColor: "#5a6e7f",
          },
        }}
      />
       <p>Ca nhiễm trong ngày: {todayCases}</p>
        </div>
        <div style={{width: "350px",height:"300px",margin:"0 auto"}}>
        <h2>Thế Giới</h2>
        <Doughnut
          data={chartDataAll}
          options={{
            legend: { display: true, position: "right" },

            datalabels: {
              display: true,
              color: "white",
            },
            tooltips: {
              backgroundColor: "#5a6e7f",
            },
          }}
       />
        <p>Ca nhiễm trong ngày: {todayCases_All}</p>
        </div>
       
    </div>
  )
}

export default BarChart