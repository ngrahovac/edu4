import React from 'react'
import './HatParam.css'

const HatParam = (props) => {

    const { text, icon } = props;

    return (
        <div
            className="flex flex-row shrink-0 tracking-wider justify uppercase text-base font-light text-slate-800">
            {icon}
            {text}
        </div>
    )
}

export default HatParam