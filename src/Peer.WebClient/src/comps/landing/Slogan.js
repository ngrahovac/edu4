import React from 'react'
import { TypeAnimation } from 'react-type-animation'

const Slogan = (props) => {
    const {
    } = props;

    return (
        <span className='text-5xl font-semibold text-left text-slate-800'>
            Find amazing&nbsp;
            <span className='font-black'>
                <TypeAnimation
                    preRenderFirstString={true}
                    sequence={[
                        'people',
                        2000,
                        'backend developers',
                        2000,
                        'telco professionals',
                        2000,
                        'mechanical engineers',
                        2000,
                        'artists',
                        2000,
                        'product designers',
                        2000,
                        'math majors',
                        2000,
                        'mentors',
                        2000
                    ]}
                    wrapper="span"
                    speed={20}
                    repeat={Infinity}
                />
            </span>
            <br/>
            and bring your ideas to life
        </span>
    )
}

export default Slogan