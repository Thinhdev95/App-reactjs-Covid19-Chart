import React,{useState} from 'react';
import {TextField, FormControl, InputLabel, Select, NativeSelect, FormHelperText, Button} from '@material-ui/core';
import { makeStyles } from '@material-ui/core/styles';
import {ChartPie} from './ChartPie';

export const Chart_Filter = (props) => {
    const [area,setArea] = useState('Asia');
    const handleSubmit =() => {
        props.changeArea(area)
      }
    return(
        <div>
        <h2>Filter Value </h2>
        <form>
            <FormControl >
            <InputLabel htmlFor="age-native-helper">Quốc gia</InputLabel>
            <NativeSelect
            value={area}
            onChange={(e) => setArea(e.target.value)}
            >
            <option value={"Asia"}>Asia</option>
            <option value={"Europe"}>Europe</option>
            <option value={"Africa"}>Africa</option>
            <option value={"North America"}>North America</option>
            <option value={"South America"}>South America</option>
            </NativeSelect>
                <FormHelperText>Some important helper text</FormHelperText>
            </FormControl>
            <br/>
            <Button onClick={() => handleSubmit()} color="primary" variant="contained">Thêm</Button>
        </form>
        <br/>
    </div>

    );    
}
export default Chart_Filter;