import re


IGS_OFFSET = 0x28

igs_offsets = []
# open GameOffsets.cs
with open('GameOffsets/IngameStateOffsets.cs', 'r') as igs:
    igs_offsets = igs.readlines()

new_igs_offsets = []
# parse the lines
for line in igs_offsets:
    # only change offsets greater than 0x300
    if not '0x' in line:
        new_igs_offsets.append(line)
        continue
    offset_value = re.search('0x[\da-fA-F]+', line)
    if offset_value and int(offset_value.group(0), 16) < 300:
        new_igs_offsets.append(line)
        continue
    new_igs_offsets.append(re.sub('0x[\dA-Fa-f]+', lambda m : '0x{:X}'.format(int(m.group(0), 16) + IGS_OFFSET, 'X'), line))

# save the file
with open('GameOffsets/IngameStateOffsets.cs', 'w') as igs:
    for line in new_igs_offsets:
        igs.write(line)
