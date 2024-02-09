import React from 'react'
import { TypeAnimation } from 'react-type-animation'

const Slogan = (props) => {
    const {
    } = props;

    const changeInterval = 2000;

    return (
        <span className='text-xl md:text-3xl lg:text-5xl font-semibold text-left text-gray-800'>
            Find amazing&nbsp;
            <span className='font-black text-indigo-500'>
                <TypeAnimation
                    preRenderFirstString={true}
                    sequence={[
                        'backend developers',
                        changeInterval,
                        'backend developers',
                        changeInterval,
                        'telco professionals',
                        changeInterval,
                        'mechanical engineers',
                        changeInterval,
                        'artists',
                        changeInterval,
                        'product designers',
                        changeInterval,
                        'math majors',
                        changeInterval,
                        'mentors',
                        changeInterval,
                        'volunteers',
                        changeInterval,
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